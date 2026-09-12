using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record IniciarEvaluacionCommand(
    Guid TemaId,
    Guid UsuarioId,
    TipoEvaluacion Tipo,
    NivelDesempeno? Nivel) : IRequest<Guid>;
