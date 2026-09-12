using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>SI el alumno falla 3 ejercicios seguidos del mismo subtema (práctica libre) ENTONCES sugerir bajar la dificultad de ese subtema.</summary>
public class ReglaBajarDificultadPorSubtema : IRegla
{
    private const int UmbralFallosConsecutivos = 3;

    public string Nombre => nameof(ReglaBajarDificultadPorSubtema);
    public int Prioridad => 10;

    public bool Evaluar(BaseDeHechos hechos) =>
        hechos.Contiene(ClavesHechos.FallosConsecutivosSubtema)
        && hechos.Obtener<int>(ClavesHechos.FallosConsecutivosSubtema) >= UmbralFallosConsecutivos;

    public void Ejecutar(BaseDeHechos hechos) =>
        hechos.Establecer(ClavesHechos.AccionSugeridaSubtema, AccionDificultad.BajarDificultad);
}
