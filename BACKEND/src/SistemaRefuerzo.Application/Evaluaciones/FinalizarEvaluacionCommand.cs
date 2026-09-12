using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record FinalizarEvaluacionCommand(Guid UsuarioId, Guid EvaluacionId) : IRequest<Guid>;
