using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record ObtenerPreguntasPorTemaQuery(Guid TemaId) : IRequest<List<PreguntaDto>>;