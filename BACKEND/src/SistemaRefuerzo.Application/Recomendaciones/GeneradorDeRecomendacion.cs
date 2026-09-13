using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;
using SistemaRefuerzo.Domain.InferenceEngine;

namespace SistemaRefuerzo.Application.Recomendaciones;

/// <summary>
/// Traduce las conclusiones dejadas por el Motor de Inferencia en la Base de Hechos
/// (nivel asignado, necesidad de refuerzo teórico, subtemas dominados y con dificultad)
/// en una <see cref="Recomendacion"/> concreta: retroalimentación en lenguaje natural
/// y ejercicios sugeridos de los subtemas donde el alumno mostró dificultad.
/// </summary>
public class GeneradorDeRecomendacion
{
    private const int MaximoEjerciciosSugeridos = 5;

    public Task<Recomendacion> GenerarAsync(
        Resultado resultado,
        Tema tema,
        IReadOnlyDictionary<Guid, Pregunta> preguntasDelTema,
        BaseDeHechos hechos,
        IReadOnlyCollection<string> reglasDisparadas,
        CancellationToken cancellationToken)
    {
        var nivel = hechos.Obtener<NivelDesempeno>(ClavesHechos.NivelAsignado);
        var requiereRefuerzoTeorico = hechos.Contiene(ClavesHechos.RequiereRefuerzoTeorico)
            && hechos.Obtener<bool>(ClavesHechos.RequiereRefuerzoTeorico);

        var subtemasConDificultad = hechos.Contiene(ClavesHechos.SubtemasConDificultad)
            ? hechos.Obtener<List<string>>(ClavesHechos.SubtemasConDificultad)
            : [];
        var subtemasDominados = hechos.Contiene(ClavesHechos.SubtemasDominados)
            ? hechos.Obtener<List<string>>(ClavesHechos.SubtemasDominados)
            : [];

        var temasPorReforzar = new List<string>(subtemasConDificultad);
        if (temasPorReforzar.Count == 0 && (nivel == NivelDesempeno.Basico || requiereRefuerzoTeorico))
            temasPorReforzar.Add(tema.Nombre);

        var recomendacion = new Recomendacion(
            resultado.Id,
            nivel,
            ConstruirRetroalimentacion(nivel, requiereRefuerzoTeorico, subtemasConDificultad, subtemasDominados),
            temasPorReforzar,
            subtemasDominados,
            reglasDisparadas);

        var ejerciciosCandidatos = preguntasDelTema.Values.Where(p => p.NivelDificultad == nivel).ToList();
        var subtemasSet = subtemasConDificultad.ToHashSet();
        var ejerciciosPriorizados = ejerciciosCandidatos.Where(p => subtemasSet.Contains(p.Subtema)).ToList();
        if (ejerciciosPriorizados.Count > 0)
            ejerciciosCandidatos = ejerciciosPriorizados;

        foreach (var ejercicio in ejerciciosCandidatos.Take(MaximoEjerciciosSugeridos))
            recomendacion.AgregarEjercicioRecomendado(ejercicio.Id);

        return Task.FromResult(recomendacion);
    }

    private static string ConstruirRetroalimentacion(
        NivelDesempeno nivel, bool requiereRefuerzoTeorico, List<string> subtemasConDificultad, List<string> subtemasDominados)
    {
        var mensaje = nivel switch
        {
            NivelDesempeno.Basico => "Tu desempeño indica que necesitas reforzar los conceptos básicos del tema.",
            NivelDesempeno.Intermedio => "Buen desempeño. Puedes continuar practicando ejercicios de nivel intermedio.",
            NivelDesempeno.Avanzado => "Excelente desempeño. Ya puedes avanzar a ejercicios de nivel avanzado.",
            _ => "No se pudo determinar una retroalimentación para el nivel obtenido.",
        };

        if (requiereRefuerzoTeorico)
            mensaje += " Además, al fallar varias preguntas seguidas, te recomendamos repasar la teoría antes de continuar.";

        if (subtemasConDificultad.Count > 0)
            mensaje += $" Presta especial atención a: {string.Join(", ", subtemasConDificultad)}.";

        if (subtemasDominados.Count > 0)
            mensaje += $" Ya dominas: {string.Join(", ", subtemasDominados)}.";

        return mensaje;
    }
}
