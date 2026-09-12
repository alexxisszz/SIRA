using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record RegistrarRespuestaCommand(Guid UsuarioId, Guid EvaluacionId, Guid PreguntaId, Guid OpcionSeleccionadaId) : IRequest;
