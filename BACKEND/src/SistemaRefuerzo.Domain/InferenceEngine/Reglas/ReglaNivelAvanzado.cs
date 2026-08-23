using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>SI puntaje &gt;= 80 ENTONCES habilitar ejercicios avanzados.</summary>
public class ReglaNivelAvanzado : IRegla
{
    public string Nombre => nameof(ReglaNivelAvanzado);
    public int Prioridad => 10;

    public bool Evaluar(BaseDeHechos hechos) =>
        hechos.Obtener<int>(ClavesHechos.Puntaje) >= 80;

    public void Ejecutar(BaseDeHechos hechos) =>
        hechos.Establecer(ClavesHechos.NivelAsignado, NivelDesempeno.Avanzado);
}