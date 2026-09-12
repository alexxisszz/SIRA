using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.InferenceEngine.Reglas;

/// <summary>SI el alumno acierta 3 ejercicios seguidos del mismo subtema (práctica libre) ENTONCES sugerir subir la dificultad de ese subtema.</summary>
public class ReglaSubirDificultadPorSubtema : IRegla
{
    private const int UmbralAciertosConsecutivos = 3;

    public string Nombre => nameof(ReglaSubirDificultadPorSubtema);
    public int Prioridad => 10;

    public bool Evaluar(BaseDeHechos hechos) =>
        hechos.Contiene(ClavesHechos.AciertosConsecutivosSubtema)
        && hechos.Obtener<int>(ClavesHechos.AciertosConsecutivosSubtema) >= UmbralAciertosConsecutivos;

    public void Ejecutar(BaseDeHechos hechos) =>
        hechos.Establecer(ClavesHechos.AccionSugeridaSubtema, AccionDificultad.SubirDificultad);
}
