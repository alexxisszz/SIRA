using MediatR;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public record ObtenerContenidoTeoricoAdminQuery(Guid TemaId) : IRequest<List<AdminContenidoTeoricoDto>>;
