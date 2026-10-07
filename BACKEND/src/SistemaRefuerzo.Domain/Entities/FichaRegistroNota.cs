using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

/// <summary>
/// Fila de la Ficha de Registro de Notas (instrumento pretest/postest) de un alumno en un tema.
/// D1 (Cognitiva) la calcula el sistema a partir de la evaluación Pretest/Postest; D2 (Procedimental)
/// y D3 (Actitudinal) las registra el docente manualmente. Todas las notas están en escala vigesimal (0-20).
/// </summary>
public class FichaRegistroNota
{
    public const decimal NotaMinima = 0m;
    public const decimal NotaMaxima = 20m;

    public Guid Id { get; private set; }
    public Guid AlumnoId { get; private set; }
    public Guid TemaId { get; private set; }
    public TipoEvaluacion TipoEvaluacion { get; private set; }

    public decimal D1I1 { get; private set; }
    public decimal D1I2 { get; private set; }
    public decimal D1I3 { get; private set; }
    public decimal D2I1 { get; private set; }
    public decimal D2I2 { get; private set; }
    public decimal D2I3 { get; private set; }
    public decimal D3I1 { get; private set; }
    public decimal D3I2 { get; private set; }
    public decimal D3I3 { get; private set; }

    public DateTime FechaActualizacion { get; private set; }

    private FichaRegistroNota() { }

    public static FichaRegistroNota Crear(Guid alumnoId, Guid temaId, TipoEvaluacion tipoEvaluacion)
    {
        if (!EsTipoValido(tipoEvaluacion))
            throw new ArgumentException("La ficha de registro de notas solo admite evaluaciones Pretest o Postest.", nameof(tipoEvaluacion));

        return new FichaRegistroNota
        {
            Id = Guid.NewGuid(),
            AlumnoId = alumnoId,
            TemaId = temaId,
            TipoEvaluacion = tipoEvaluacion,
            FechaActualizacion = DateTime.UtcNow,
        };
    }

    public static bool EsTipoValido(TipoEvaluacion tipoEvaluacion) =>
        tipoEvaluacion is TipoEvaluacion.Pretest or TipoEvaluacion.Postest;

    /// <summary>Indica si ya se registró la dimensión cognitiva (alguna nota de D1 mayor que 0).</summary>
    public bool TieneD1 => D1I1 + D1I2 + D1I3 > 0;

    public void ActualizarD1(decimal i1, decimal i2, decimal i3)
    {
        ValidarNotas(i1, i2, i3);

        D1I1 = i1;
        D1I2 = i2;
        D1I3 = i3;
        FechaActualizacion = DateTime.UtcNow;
    }

    public void ActualizarD2D3(decimal d2i1, decimal d2i2, decimal d2i3, decimal d3i1, decimal d3i2, decimal d3i3)
    {
        ValidarNotas(d2i1, d2i2, d2i3, d3i1, d3i2, d3i3);

        D2I1 = d2i1;
        D2I2 = d2i2;
        D2I3 = d2i3;
        D3I1 = d3i1;
        D3I2 = d3i2;
        D3I3 = d3i3;
        FechaActualizacion = DateTime.UtcNow;
    }

    private static void ValidarNotas(params decimal[] notas)
    {
        if (notas.Any(n => n < NotaMinima || n > NotaMaxima))
            throw new ArgumentOutOfRangeException(nameof(notas), $"Las notas deben estar entre {NotaMinima} y {NotaMaxima}.");
    }
}
