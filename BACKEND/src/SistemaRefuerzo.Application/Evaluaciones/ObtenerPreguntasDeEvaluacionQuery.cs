using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record ObtenerPreguntasDeEvaluacionQuery(Guid EvaluacionId, Guid UsuarioId) : IRequest<List<PreguntaDto>>;
