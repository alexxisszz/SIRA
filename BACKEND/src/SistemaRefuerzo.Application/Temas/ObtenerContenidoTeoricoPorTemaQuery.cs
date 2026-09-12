using MediatR;

namespace SistemaRefuerzo.Application.Temas;

public record ObtenerContenidoTeoricoPorTemaQuery(Guid TemaId) : IRequest<List<ContenidoTeoricoDto>>;
