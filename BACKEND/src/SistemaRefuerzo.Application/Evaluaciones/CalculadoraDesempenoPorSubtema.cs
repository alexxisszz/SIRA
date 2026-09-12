using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Evaluaciones;

/// <summary>
/// Calcula el % de aciertos por subtema a partir de las respuestas de una evaluación,
/// dado el diccionario de preguntas del tema (PreguntaId → Pregunta). Compartido entre
/// el cálculo de hechos al finalizar una evaluación y el reporte de progreso diagnóstico/final.
/// </summary>
public static class CalculadoraDesempenoPorSubtema
{
    public static Dictionary<string, double> Calcular(
        IEnumerable<RespuestaAlumno> respuestas, IReadOnlyDictionary<Guid, Pregunta> preguntasDelTema)
    {
        return respuestas
            .Where(r => preguntasDelTema.ContainsKey(r.PreguntaId))
            .GroupBy(r => preguntasDelTema[r.PreguntaId].Subtema)
            .ToDictionary(g => g.Key, g => g.Count(r => r.EsCorrecta) * 100.0 / g.Count());
    }
}
