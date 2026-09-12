using MediatR;

namespace SistemaRefuerzo.Application.Temas;

public record ObtenerVideosPorTemaQuery(Guid TemaId) : IRequest<List<VideoApoyoDto>>;
