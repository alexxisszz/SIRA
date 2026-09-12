using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class IniciarEvaluacionCommandHandler(
    ITemaRepository temaRepository,
    IUsuarioRepository usuarioRepository,
    IEvaluacionRepository evaluacionRepository,
    IPreguntaRepository preguntaRepository,
    ProgresoTemaService progresoTemaService,
    IUnitOfWork unitOfWork) : IRequestHandler<IniciarEvaluacionCommand, Guid>
{
    private static readonly TimeSpan TiempoEsperaEntreIntentos = TimeSpan.FromHours(24);
    private const int CantidadPreguntasPorEvaluacion = 10;

    public async Task<Guid> Handle(IniciarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        if (!await progresoTemaService.TemaDesbloqueadoAsync(tema, alumno.Id, cancellationToken))
            throw new ReglaDeNegocioException("Debes aprobar la prueba final del tema anterior para acceder a este tema.");

        await ValidarRequisitoPrevioAsync(request, alumno.Id, cancellationToken);

        var ultimaEvaluacion = await evaluacionRepository.ObtenerUltimaFinalizadaAsync(
            alumno.Id, tema.Id, request.Tipo, request.Nivel, cancellationToken);
        if (ultimaEvaluacion?.FechaFin is DateTime fechaFin)
        {
            var proximoIntentoDisponible = fechaFin.Add(TiempoEsperaEntreIntentos);
            if (proximoIntentoDisponible > DateTime.UtcNow)
            {
                var horasRestantes = Math.Ceiling((proximoIntentoDisponible - DateTime.UtcNow).TotalHours);
                throw new ReglaDeNegocioException(
                    $"Ya realizaste esta evaluación recientemente. Podrás repetirla en aproximadamente {horasRestantes} hora(s).");
            }
        }

        var preguntasDisponibles = request.Nivel is NivelDesempeno nivelPool
            ? await preguntaRepository.ObtenerPorTemaYNivelAsync(tema.Id, nivelPool, cancellationToken)
            : await preguntaRepository.ObtenerPorTemaAsync(tema.Id, cancellationToken);

        var preguntasAsignadas = preguntasDisponibles
            .OrderBy(_ => Guid.NewGuid())
            .Take(CantidadPreguntasPorEvaluacion)
            .Select(p => p.Id);

        var evaluacion = new Evaluacion(tema.Id, alumno.Id, request.Tipo, preguntasAsignadas, request.Nivel);

        evaluacionRepository.Agregar(evaluacion);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return evaluacion.Id;
    }

    private async Task ValidarRequisitoPrevioAsync(IniciarEvaluacionCommand request, Guid alumnoId, CancellationToken cancellationToken)
    {
        switch (request.Tipo)
        {
            case TipoEvaluacion.Diagnostica:
                return;

            case TipoEvaluacion.PorNivel when request.Nivel == NivelDesempeno.Basico:
                var diagnostico = await evaluacionRepository.ObtenerUltimaFinalizadaAsync(
                    alumnoId, request.TemaId, TipoEvaluacion.Diagnostica, null, cancellationToken);
                if (diagnostico is null)
                    throw new ReglaDeNegocioException("Debes completar la prueba de entrada del tema antes de rendir una evaluación por nivel.");
                return;

            case TipoEvaluacion.PorNivel when request.Nivel == NivelDesempeno.Intermedio:
                await ExigirAprobacionAsync(alumnoId, request.TemaId, NivelDesempeno.Basico, "Básico", cancellationToken);
                return;

            case TipoEvaluacion.PorNivel when request.Nivel == NivelDesempeno.Avanzado:
                await ExigirAprobacionAsync(alumnoId, request.TemaId, NivelDesempeno.Intermedio, "Intermedio", cancellationToken);
                return;

            case TipoEvaluacion.Final:
                await ExigirAprobacionAsync(alumnoId, request.TemaId, NivelDesempeno.Avanzado, "Avanzado", cancellationToken);
                return;

            default:
                return;
        }
    }

    private async Task ExigirAprobacionAsync(
        Guid alumnoId, Guid temaId, NivelDesempeno nivelRequerido, string nombreNivel, CancellationToken cancellationToken)
    {
        var aprobado = await progresoTemaService.EstaAprobadaAsync(
            alumnoId, temaId, TipoEvaluacion.PorNivel, nivelRequerido, ProgresoTemaService.PuntajeAprobacionNivel, cancellationToken);
        if (!aprobado)
            throw new ReglaDeNegocioException($"Debes aprobar el nivel {nombreNivel} antes de continuar.");
    }
}
