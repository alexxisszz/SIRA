using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Evaluaciones;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Recomendaciones;

public class ObtenerRecomendacionQueryHandler(
    IUsuarioRepository usuarioRepository,
    IRecomendacionRepository recomendacionRepository,
    IResultadoRepository resultadoRepository,
    IEvaluacionRepository evaluacionRepository,
    IPreguntaRepository preguntaRepository) : IRequestHandler<ObtenerRecomendacionQuery, RecomendacionDto>
{
    public async Task<RecomendacionDto> Handle(ObtenerRecomendacionQuery request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var recomendacion = await recomendacionRepository.ObtenerPorIdAsync(request.RecomendacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Recomendacion), request.RecomendacionId);

        var resultado = await resultadoRepository.ObtenerPorIdAsync(recomendacion.ResultadoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Resultado), recomendacion.ResultadoId);

        var evaluacion = await evaluacionRepository.ObtenerPorIdAsync(resultado.EvaluacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Evaluacion), resultado.EvaluacionId);

        if (evaluacion.AlumnoId != alumno.Id)
            throw new NotFoundException(nameof(Recomendacion), request.RecomendacionId);

        var ejercicios = new List<EjercicioSugeridoDto>();
        foreach (var ejercicio in recomendacion.EjerciciosRecomendados)
        {
            var pregunta = await preguntaRepository.ObtenerPorIdAsync(ejercicio.PreguntaId, cancellationToken);
            if (pregunta is not null)
                ejercicios.Add(new EjercicioSugeridoDto(pregunta.Id, pregunta.Enunciado));
        }

        var respuestasDetalle = new List<RespuestaDetalleDto>();
        foreach (var respuesta in evaluacion.Respuestas)
        {
            var pregunta = await preguntaRepository.ObtenerPorIdAsync(respuesta.PreguntaId, cancellationToken);
            if (pregunta is null)
                continue;

            var opcionSeleccionada = pregunta.Opciones.FirstOrDefault(o => o.Id == respuesta.OpcionSeleccionadaId);
            var opcionCorrecta = pregunta.Opciones.FirstOrDefault(o => o.EsCorrecta);

            respuestasDetalle.Add(new RespuestaDetalleDto(
                pregunta.Id,
                pregunta.Enunciado,
                opcionSeleccionada?.Texto ?? string.Empty,
                opcionCorrecta?.Texto ?? string.Empty,
                respuesta.EsCorrecta));
        }

        var reporteProgreso = evaluacion.Tipo == TipoEvaluacion.Final
            ? await ArmarReporteProgresoAsync(evaluacion, resultado, recomendacion.Nivel, cancellationToken)
            : null;

        return new RecomendacionDto(
            recomendacion.Id,
            evaluacion.TemaId,
            resultado.Puntaje,
            recomendacion.Nivel,
            recomendacion.TemasPorReforzar.ToList(),
            recomendacion.SubtemasDominados.ToList(),
            ejercicios,
            recomendacion.Retroalimentacion,
            respuestasDetalle,
            reporteProgreso);
    }

    private async Task<ReporteProgresoTemaDto?> ArmarReporteProgresoAsync(
        Evaluacion evaluacionFinal, Resultado resultadoFinal, NivelDesempeno nivelFinal, CancellationToken cancellationToken)
    {
        var diagnostico = await evaluacionRepository.ObtenerUltimaFinalizadaAsync(
            evaluacionFinal.AlumnoId, evaluacionFinal.TemaId, TipoEvaluacion.Diagnostica, null, cancellationToken);
        if (diagnostico is null)
            return null;

        var resultadoDiagnostico = await resultadoRepository.ObtenerPorEvaluacionIdAsync(diagnostico.Id, cancellationToken);
        if (resultadoDiagnostico is null)
            return null;

        var recomendacionDiagnostico = await recomendacionRepository.ObtenerPorResultadoIdAsync(resultadoDiagnostico.Id, cancellationToken);

        var preguntasDelTema = (await preguntaRepository.ObtenerPorTemaAsync(evaluacionFinal.TemaId, cancellationToken))
            .ToDictionary(p => p.Id);

        var desempenoInicial = CalculadoraDesempenoPorSubtema.Calcular(diagnostico.Respuestas, preguntasDelTema);
        var desempenoFinal = CalculadoraDesempenoPorSubtema.Calcular(evaluacionFinal.Respuestas, preguntasDelTema);

        var subtemasComparados = desempenoInicial.Keys
            .Intersect(desempenoFinal.Keys)
            .Select(subtema => new ProgresoSubtemaComparadoDto(subtema, desempenoInicial[subtema], desempenoFinal[subtema]))
            .ToList();

        return new ReporteProgresoTemaDto(
            resultadoDiagnostico.Puntaje,
            resultadoFinal.Puntaje,
            recomendacionDiagnostico?.Nivel,
            nivelFinal,
            subtemasComparados);
    }
}
