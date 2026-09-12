namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>
/// SI existe un desempeño por subtema registrado ENTONCES clasificar cada subtema como
/// dominado o con dificultad, según si el % de aciertos alcanza el umbral de dominio.
/// Es la regla que permite al motor identificar, además del nivel general, en qué
/// subtemas concretos falla o destaca el alumno (requisito del diagnóstico adaptativo).
/// </summary>
public class ReglaAnalisisSubtemas : IRegla
{
    private const double PorcentajeMinimoDominio = 60.0;

    public string Nombre => nameof(ReglaAnalisisSubtemas);
    public int Prioridad => 15;

    public bool Evaluar(BaseDeHechos hechos) => hechos.Contiene(ClavesHechos.DesempenoPorSubtema);

    public void Ejecutar(BaseDeHechos hechos)
    {
        var desempenoPorSubtema = hechos.Obtener<Dictionary<string, double>>(ClavesHechos.DesempenoPorSubtema);

        var subtemasConDificultad = desempenoPorSubtema
            .Where(par => par.Value < PorcentajeMinimoDominio)
            .OrderBy(par => par.Value)
            .Select(par => par.Key)
            .ToList();

        var subtemasDominados = desempenoPorSubtema
            .Where(par => par.Value >= PorcentajeMinimoDominio)
            .OrderByDescending(par => par.Value)
            .Select(par => par.Key)
            .ToList();

        hechos.Establecer(ClavesHechos.SubtemasConDificultad, subtemasConDificultad);
        hechos.Establecer(ClavesHechos.SubtemasDominados, subtemasDominados);
    }
}
