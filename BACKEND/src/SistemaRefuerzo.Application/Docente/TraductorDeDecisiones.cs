using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record DecisionSistemaDto(string ReglaAplicada, string Motivo, string Accion);

/// <summary>
/// Traduce los nombres internos de las reglas del motor de inferencia (guardados en
/// <see cref="Domain.Entities.Recomendacion.ReglasAplicadas"/>) a lenguaje comprensible
/// para un docente, sin exponer nombres de clases C# ni detalles técnicos.
/// </summary>
public static class TraductorDeDecisiones
{
    public static List<DecisionSistemaDto> Traducir(
        IReadOnlyCollection<string> reglasAplicadas,
        NivelDesempeno nivel,
        int puntaje,
        int fallosConsecutivos,
        IReadOnlyCollection<string> subtemasConDificultad,
        string temaNombre)
    {
        var decisiones = new List<DecisionSistemaDto>();

        foreach (var regla in reglasAplicadas)
        {
            var decision = regla switch
            {
                "ReglaNivelBasico" => new DecisionSistemaDto(
                    "Asignación de nivel básico",
                    $"El estudiante obtuvo {puntaje}/100 en \"{temaNombre}\", un puntaje bajo para este tema.",
                    "Asignar ejercicios de nivel básico y reforzar los conceptos fundamentales."),

                "ReglaNivelIntermedio" => new DecisionSistemaDto(
                    "Asignación de nivel intermedio",
                    $"El estudiante obtuvo {puntaje}/100 en \"{temaNombre}\", un desempeño aceptable.",
                    "Asignar ejercicios de nivel intermedio para consolidar el tema."),

                "ReglaNivelAvanzado" => new DecisionSistemaDto(
                    "Asignación de nivel avanzado",
                    $"El estudiante obtuvo {puntaje}/100 en \"{temaNombre}\", un desempeño sobresaliente.",
                    "Habilitar ejercicios de nivel avanzado."),

                "ReglaRefuerzoTeorico" => new DecisionSistemaDto(
                    "Refuerzo teórico",
                    $"El estudiante encadenó {fallosConsecutivos} fallos consecutivos en \"{temaNombre}\".",
                    "Mostrar contenido teórico de repaso antes de continuar con más ejercicios."),

                "ReglaAnalisisSubtemas" when subtemasConDificultad.Count > 0 => new DecisionSistemaDto(
                    $"Refuerzo de {string.Join(", ", subtemasConDificultad)}",
                    $"El estudiante tuvo un rendimiento inferior al esperado en: {string.Join(", ", subtemasConDificultad)}.",
                    "Priorizar ejercicios sugeridos de esos subtemas antes de avanzar."),

                _ => null,
            };

            if (decision is not null)
                decisiones.Add(decision);
        }

        return decisiones;
    }
}
