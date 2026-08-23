namespace SistemaRefuerzo.Application.Reportes.Docente;

public record EstadisticaPorNivelDto(string Nivel, int Cantidad);

public record EstadisticaPorTemaDto(
    string TemaNombre,
    int EvaluacionesRealizadas,
    double PuntajePromedio,
    List<EstadisticaPorNivelDto> DistribucionNiveles);

public record EstadisticasDto(
    int TotalEvaluaciones,
    double PuntajePromedioGeneral,
    List<EstadisticaPorTemaDto> PorTema);
