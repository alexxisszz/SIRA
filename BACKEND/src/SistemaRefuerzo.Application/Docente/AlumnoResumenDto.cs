using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record AlumnoResumenDto(
    Guid AlumnoId,
    string Nombres,
    string Apellidos,
    string Grado,
    int EvaluacionesRealizadas,
    NivelDesempeno? UltimoNivel,
    DateTime? UltimaEvaluacion);
