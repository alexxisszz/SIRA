using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record RegistrarRespuestaCommand(Guid EvaluacionId, Guid PreguntaId, Guid OpcionSeleccionadaId) : IRequest;