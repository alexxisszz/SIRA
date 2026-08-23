using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>SI puntaje &gt;= 50 Y puntaje &lt; 80 ENTONCES asignar ejercicios intermedios.</summary>
public class ReglaNivelIntermedio : IRegla
{
    public string Nombre => nameof(ReglaNivelIntermedio);
    public int Prioridad => 10;

    public bool Evaluar(BaseDeHechos hechos)
    {
        var puntaje = hechos.Obtener<int>(ClavesHechos.Puntaje);
        return puntaje >= 50 && puntaje < 80;
    }

    public void Ejecutar(BaseDeHechos hechos) =>
        hechos.Establecer(ClavesHechos.NivelAsignado, NivelDesempeno.Intermedio);
}