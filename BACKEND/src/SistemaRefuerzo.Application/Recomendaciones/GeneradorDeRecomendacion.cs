using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;
using SistemaRefuerzo.Domain.InferenceEngine;

namespace SistemaRefuerzo.Application.Recomendaciones;

/// <summary>
/// Traduce las conclusiones dejadas por el Motor de Inferencia en la Base de Hechos
/// (nivel asignado, necesidad de refuerzo teórico) en una <see cref="Recomendacion"/>
/// concreta: retroalimentación en lenguaje natural, temas a reforzar y ejercicios sugeridos.
/// </summary>
public class GeneradorDeRecomendacion(IPreguntaRepository preguntaRepository)
{
    private const int MaximoEjerciciosSugeridos = 5;

    public async Task<Recomendacion> GenerarAsync(
        Resultado resultado,
        Tema tema,
        BaseDeHechos hechos,
        CancellationToken cancellationToken)
    {
        var nivel = hechos.Obtener<NivelDesempeno>(ClavesHechos.NivelAsignado);
        var requiereRefuerzoTeorico = hechos.Contiene(ClavesHechos.RequiereRefuerzoTeorico)
            && hechos.Obtener<bool>(ClavesHechos.RequiereRefuerzoTeorico);

        var temasPorReforzar = new List<string>();
        if (nivel == NivelDesempeno.Basico || requiereRefuerzoTeorico)
            temasPorReforzar.Add(tema.Nombre);

        var recomendacion = new Recomendacion(
            resultado.Id,
            nivel,
            ConstruirRetroalimentacion(nivel, requiereRefuerzoTeorico),
            temasPorReforzar);

        var ejerciciosSugeridos = await preguntaRepository.ObtenerPorTemaYNivelAsync(tema.Id, nivel, cancellationToken);
        foreach (var ejercicio in ejerciciosSugeridos.Take(MaximoEjerciciosSugeridos))
            recomendacion.AgregarEjercicioRecomendado(ejercicio.Id);

        return recomendacion;
    }

    private static string ConstruirRetroalimentacion(NivelDesempeno nivel, bool requiereRefuerzoTeorico)
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

        return mensaje;
    }
}