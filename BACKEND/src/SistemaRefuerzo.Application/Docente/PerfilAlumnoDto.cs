using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record SubtemaRendimientoDto(string Subtema, double PorcentajeAciertos, string Estado);

public record PuntoEvolucionDto(DateTime Fecha, string Etiqueta, int Puntaje);

public record PerfilRendimientoAlumnoDto(
    Guid AlumnoId,
    string Nombres,
    string Apellidos,
    string Grado,
    string? TemaActualNombre,
    DateTime? UltimaActividad,
    NivelDesempeno? NivelInicial,
    NivelDesempeno? NivelActual,
    string Evolucion,
    int? UltimaNota,
    double? PorcentajeAciertosGlobal,
    int CantidadEvaluaciones,
    List<SubtemaRendimientoDto> RendimientoPorSubtema,
    List<string> Fortalezas,
    List<string> Dificultades,
    string? RecomendacionActual,
    List<DecisionSistemaDto> DecisionesSistema,
    List<PuntoEvolucionDto> EvolucionPuntajes,
    List<ResultadoHistoricoDto> Historial);
