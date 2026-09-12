using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;
using SistemaRefuerzo.Domain.InferenceEngine;

namespace SistemaRefuerzo.Application.Ejercicios;

public class RegistrarIntentoCommandHandler(
    IUsuarioRepository usuarioRepository,
    IPreguntaRepository preguntaRepository,
    IIntentoEjercicioRepository intentoEjercicioRepository,
    IReglaRepository reglaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegistrarIntentoCommand, IntentoResultadoDto>
{
    private const int CantidadHistorialParaRacha = 10;

    public async Task<IntentoResultadoDto> Handle(RegistrarIntentoCommand request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var pregunta = await preguntaRepository.ObtenerPorIdAsync(request.PreguntaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Pregunta), request.PreguntaId);

        var opcionSeleccionada = pregunta.Opciones.FirstOrDefault(o => o.Id == request.OpcionSeleccionadaId)
            ?? throw new NotFoundException(nameof(OpcionPregunta), request.OpcionSeleccionadaId);

        var opcionCorrecta = pregunta.Opciones.First(o => o.EsCorrecta);

        var intento = new IntentoEjercicio(
            alumno.Id, pregunta.TemaId, pregunta.Subtema, pregunta.Id, opcionSeleccionada.Id, opcionSeleccionada.EsCorrecta);
        intentoEjercicioRepository.Agregar(intento);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        // 1. El historial reciente del alumno en este subtema se traduce a HECHOS (rachas).
        var historial = await intentoEjercicioRepository.ObtenerUltimosPorAlumnoYSubtemaAsync(
            alumno.Id, pregunta.Subtema, CantidadHistorialParaRacha, cancellationToken);

        var hechos = new BaseDeHechos();
        hechos.Establecer(ClavesHechos.AciertosConsecutivosSubtema, ContarRachaConsecutiva(historial, esCorrecta: true));
        hechos.Establecer(ClavesHechos.FallosConsecutivosSubtema, ContarRachaConsecutiva(historial, esCorrecta: false));

        // 2. Se cargan las reglas activas de la BASE DE CONOCIMIENTO (las que no aplican a este
        //    contexto simplemente no encuentran sus hechos y no se disparan) y se resuelven a IRegla.
        var reglasActivas = await reglaRepository.ObtenerActivasAsync(cancellationToken);
        var reglasEjecutables = reglasActivas.Select(r => RegistroReglas.Resolver(r.NombreClaseRegla));

        // 3. El MOTOR DE INFERENCIA concluye si corresponde subir/bajar la dificultad del subtema.
        new MotorInferencia().Ejecutar(hechos, reglasEjecutables);

        var accionSugerida = hechos.Contiene(ClavesHechos.AccionSugeridaSubtema)
            ? hechos.Obtener<AccionDificultad>(ClavesHechos.AccionSugeridaSubtema)
            : AccionDificultad.Ninguna;

        return new IntentoResultadoDto(opcionSeleccionada.EsCorrecta, opcionCorrecta.Id, pregunta.Explicacion, pregunta.Subtema, accionSugerida);
    }

    private static int ContarRachaConsecutiva(List<IntentoEjercicio> historialDesc, bool esCorrecta)
    {
        var racha = 0;
        foreach (var intento in historialDesc)
        {
            if (intento.EsCorrecta != esCorrecta)
                break;

            racha++;
        }

        return racha;
    }
}
