using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record FinalizarEvaluacionCommand(Guid EvaluacionId) : IRequest<Guid>;