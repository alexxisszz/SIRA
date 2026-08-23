using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>SI puntaje &lt; 50 ENTONCES asignar ejercicios básicos.</summary>
public class ReglaNivelBasico : IRegla
{
    public string Nombre => nameof(ReglaNivelBasico);
    public int Prioridad => 10;

    public bool Evaluar(BaseDeHechos hechos) =>
        hechos.Obtener<int>(ClavesHechos.Puntaje) < 50;

    public void Ejecutar(BaseDeHechos hechos) =>
        hechos.Establecer(ClavesHechos.NivelAsignado, NivelDesempeno.Basico);
}