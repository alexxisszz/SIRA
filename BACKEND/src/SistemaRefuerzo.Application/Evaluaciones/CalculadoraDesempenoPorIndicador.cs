using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

/// <summary>
/// Calcula la nota por indicador cognitivo (I1, I2, I3) de la dimensión D1 de la Ficha de Registro
/// de Notas a partir de las respuestas de una evaluación Pretest/Postest, dado el diccionario de
/// preguntas del tema (PreguntaId → Pregunta). La nota se expresa en escala vigesimal (0-20), que es
/// la escala del instrumento; un indicador sin preguntas en la evaluación queda en 0.
/// </summary>
public static class CalculadoraDesempenoPorIndicador
{
    private const decimal EscalaFicha = FichaRegistroNota.NotaMaxima;

    public static Dictionary<IndicadorCognitivo, decimal> Calcular(
        IEnumerable<RespuestaAlumno> respuestas, IReadOnlyDictionary<Guid, Pregunta> preguntasDelTema)
    {
        var notasPorIndicador = respuestas
            .Where(r => preguntasDelTema.ContainsKey(r.PreguntaId))
            .GroupBy(r => preguntasDelTema[r.PreguntaId].Indicador)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(g.Count(r => r.EsCorrecta) * EscalaFicha / g.Count(), 2));

        foreach (var indicador in Enum.GetValues<IndicadorCognitivo>())
            notasPorIndicador.TryAdd(indicador, 0m);

        return notasPorIndicador;
    }
}
