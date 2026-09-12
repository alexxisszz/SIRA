using MediatR;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public record EliminarContenidoTeoricoCommand(Guid ContenidoId) : IRequest;
