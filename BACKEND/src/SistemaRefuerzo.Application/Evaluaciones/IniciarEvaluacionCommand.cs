using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record IniciarEvaluacionCommand(Guid TemaId, Guid UsuarioId) : IRequest<Guid>;