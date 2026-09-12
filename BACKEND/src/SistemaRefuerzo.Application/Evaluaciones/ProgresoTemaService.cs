using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

/// <summary>
/// Reglas de avance del alumno dentro de un tema: cada nivel se desbloquea al aprobar
/// el anterior, y un tema solo se desbloquea si el tema previo (según su Orden) ya
/// tiene la prueba final aprobada. Los umbrales de aprobación son una decisión de
/// producto explícita (no forman parte del motor de inferencia de recomendaciones).
/// </summary>
public class ProgresoTemaService(
    ITemaRepository temaRepository,
    IEvaluacionRepository evaluacionRepository,
    IResultadoRepository resultadoRepository)
{
    public const int PuntajeAprobacionNivel = 60;
    public const int PuntajeAprobacionFinal = 70;

    public async Task<bool> EstaAprobadaAsync(
        Guid alumnoId, Guid temaId, TipoEvaluacion tipo, NivelDesempeno? nivel, int puntajeMinimo, CancellationToken cancellationToken)
    {
        var evaluacion = await evaluacionRepository.ObtenerUltimaFinalizadaAsync(alumnoId, temaId, tipo, nivel, cancellationToken);
        if (evaluacion is null)
            return false;

        var resultado = await resultadoRepository.ObtenerPorEvaluacionIdAsync(evaluacion.Id, cancellationToken);
        return resultado is not null && resultado.Puntaje >= puntajeMinimo;
    }

    public async Task<bool> TemaDesbloqueadoAsync(Tema tema, Guid alumnoId, CancellationToken cancellationToken)
    {
        if (tema.Orden <= 1)
            return true;

        var temas = await temaRepository.ObtenerTodosAsync(cancellationToken);
        var temaAnterior = temas.FirstOrDefault(t => t.Orden == tema.Orden - 1);
        if (temaAnterior is null)
            return true;

        return await EstaAprobadaAsync(alumnoId, temaAnterior.Id, TipoEvaluacion.Final, null, PuntajeAprobacionFinal, cancellationToken);
    }
}
