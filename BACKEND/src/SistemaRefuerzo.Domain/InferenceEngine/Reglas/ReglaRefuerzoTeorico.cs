namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>SI el estudiante falla tres veces consecutivas ENTONCES mostrar teoría y ejercicios de refuerzo.</summary>
public class ReglaRefuerzoTeorico : IRegla
{
    private const int UmbralFallosConsecutivos = 3;

    public string Nombre => nameof(ReglaRefuerzoTeorico);
    public int Prioridad => 20;

    public bool Evaluar(BaseDeHechos hechos) =>
        hechos.Obtener<int>(ClavesHechos.FallosConsecutivos) >= UmbralFallosConsecutivos;

    public void Ejecutar(BaseDeHechos hechos) =>
        hechos.Establecer(ClavesHechos.RequiereRefuerzoTeorico, true);
}