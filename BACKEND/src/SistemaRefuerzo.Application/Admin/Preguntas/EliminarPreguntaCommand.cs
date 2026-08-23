using MediatR;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public record EliminarPreguntaCommand(Guid PreguntaId) : IRequest;
