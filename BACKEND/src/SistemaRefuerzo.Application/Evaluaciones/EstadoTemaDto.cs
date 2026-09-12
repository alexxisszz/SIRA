namespace SistemaRefuerzo.Application.Evaluaciones;

public record EstadoTemaDto(
    bool TemaDesbloqueado,
    bool DiagnosticoCompletado,
    bool BasicoAprobado,
    bool IntermedioAprobado,
    bool AvanzadoAprobado,
    bool FinalAprobada);
