using SistemaRefuerzo.Domain.InferenceEngine.Reglas;

namespace SistemaRefuerzo.Domain.InferenceEngine;

/// <summary>
/// Traduce el nombre de clase almacenado en la entidad <c>Regla</c> (Base de Conocimiento
/// persistida) a la implementación ejecutable de <see cref="IRegla"/> correspondiente.
/// Añadir una regla nueva implica: crear la clase que implementa IRegla y registrarla aquí.
/// </summary>
public static class RegistroReglas
{
    private static readonly Dictionary<string, Func<IRegla>> Reglas = new()
    {
        [nameof(ReglaNivelBasico)] = () => new ReglaNivelBasico(),
        [nameof(ReglaNivelIntermedio)] = () => new ReglaNivelIntermedio(),
        [nameof(ReglaNivelAvanzado)] = () => new ReglaNivelAvanzado(),
        [nameof(ReglaRefuerzoTeorico)] = () => new ReglaRefuerzoTeorico(),
    };

    public static IRegla Resolver(string nombreClaseRegla)
    {
        if (!Reglas.TryGetValue(nombreClaseRegla, out var fabrica))
            throw new InvalidOperationException($"No existe una implementación registrada para la regla '{nombreClaseRegla}'.");

        return fabrica();
    }
}