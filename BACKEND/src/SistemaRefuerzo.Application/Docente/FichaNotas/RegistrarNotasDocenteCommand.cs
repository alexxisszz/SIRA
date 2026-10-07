using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record RegistrarNotasDocenteCommand(
    Guid AlumnoId,
    Guid TemaId,
    TipoEvaluacion TipoEvaluacion,
    decimal D2I1,
    decimal D2I2,
    decimal D2I3,
    decimal D3I1,
    decimal D3I2,
    decimal D3I3) : IRequest;
