using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

/// <summary>Notas de una dimensión (I1, I2, I3) y su promedio P = (I1 + I2 + I3) / 3, en escala 0-20.</summary>
public record DimensionFichaDto(decimal I1, decimal I2, decimal I3, decimal Promedio);

/// <summary>
/// Fila de la ficha para un alumno. Si el alumno todavía no tiene notas registradas para el
/// tema/tipo, Registrada es false y las dimensiones y el promedio final son null.
/// </summary>
public record FichaAlumnoDto(
    Guid AlumnoId,
    string Nombres,
    string Apellidos,
    bool Registrada,
    DimensionFichaDto? D1,
    DimensionFichaDto? D2,
    DimensionFichaDto? D3,
    decimal? PromedioFinal,
    DateTime? FechaActualizacion);

public record FichaRegistroNotasDto(
    Guid TemaId,
    string TemaNombre,
    TipoEvaluacion TipoEvaluacion,
    List<FichaAlumnoDto> Alumnos);
