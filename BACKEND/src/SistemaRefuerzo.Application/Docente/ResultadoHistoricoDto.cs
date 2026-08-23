using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ResultadoHistoricoDto(
    Guid EvaluacionId,
    string TemaNombre,
    int Puntaje,
    int FallosConsecutivos,
    DateTime FechaCalculo,
    NivelDesempeno Nivel,
    string Retroalimentacion);
