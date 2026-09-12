using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class ObtenerEstadoTemaQueryHandler(
    ITemaRepository temaRepository,
    IUsuarioRepository usuarioRepository,
    IEvaluacionRepository evaluacionRepository,
    ProgresoTemaService progresoTemaService) : IRequestHandler<ObtenerEstadoTemaQuery, EstadoTemaDto>
{
    public async Task<EstadoTemaDto> Handle(ObtenerEstadoTemaQuery request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var temaDesbloqueado = await progresoTemaService.TemaDesbloqueadoAsync(tema, alumno.Id, cancellationToken);

        var diagnostico = await evaluacionRepository.ObtenerUltimaFinalizadaAsync(
            alumno.Id, tema.Id, TipoEvaluacion.Diagnostica, null, cancellationToken);

        var basicoAprobado = await progresoTemaService.EstaAprobadaAsync(
            alumno.Id, tema.Id, TipoEvaluacion.PorNivel, NivelDesempeno.Basico, ProgresoTemaService.PuntajeAprobacionNivel, cancellationToken);
        var intermedioAprobado = await progresoTemaService.EstaAprobadaAsync(
            alumno.Id, tema.Id, TipoEvaluacion.PorNivel, NivelDesempeno.Intermedio, ProgresoTemaService.PuntajeAprobacionNivel, cancellationToken);
        var avanzadoAprobado = await progresoTemaService.EstaAprobadaAsync(
            alumno.Id, tema.Id, TipoEvaluacion.PorNivel, NivelDesempeno.Avanzado, ProgresoTemaService.PuntajeAprobacionNivel, cancellationToken);
        var finalAprobada = await progresoTemaService.EstaAprobadaAsync(
            alumno.Id, tema.Id, TipoEvaluacion.Final, null, ProgresoTemaService.PuntajeAprobacionFinal, cancellationToken);

        return new EstadoTemaDto(
            temaDesbloqueado,
            diagnostico is not null,
            basicoAprobado,
            intermedioAprobado,
            avanzadoAprobado,
            finalAprobada);
    }
}
