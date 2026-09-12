namespace SistemaRefuerzo.Domain.InferenceEngine;

public static class ClavesHechos
{
    public const string Puntaje = "Puntaje";
    public const string FallosConsecutivos = "FallosConsecutivos";
    public const string NivelAsignado = "NivelAsignado";
    public const string RequiereRefuerzoTeorico = "RequiereRefuerzoTeorico";

    /// <summary>Dictionary&lt;string, double&gt;: % de aciertos del alumno en esta evaluación, por subtema.</summary>
    public const string DesempenoPorSubtema = "DesempenoPorSubtema";

    /// <summary>List&lt;string&gt;: subtemas donde el % de aciertos quedó por debajo del umbral de dominio.</summary>
    public const string SubtemasConDificultad = "SubtemasConDificultad";

    /// <summary>List&lt;string&gt;: subtemas donde el % de aciertos alcanzó o superó el umbral de dominio.</summary>
    public const string SubtemasDominados = "SubtemasDominados";

    /// <summary>int: aciertos consecutivos del alumno en el subtema del ejercicio recién resuelto (práctica libre).</summary>
    public const string AciertosConsecutivosSubtema = "AciertosConsecutivosSubtema";

    /// <summary>int: fallos consecutivos del alumno en el subtema del ejercicio recién resuelto (práctica libre).</summary>
    public const string FallosConsecutivosSubtema = "FallosConsecutivosSubtema";

    /// <summary>AccionDificultad: conclusión del motor sobre si subir/bajar la dificultad del subtema en práctica.</summary>
    public const string AccionSugeridaSubtema = "AccionSugeridaSubtema";
}
