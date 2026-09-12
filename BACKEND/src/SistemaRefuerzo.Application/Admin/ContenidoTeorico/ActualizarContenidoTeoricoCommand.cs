using MediatR;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public record ActualizarContenidoTeoricoCommand(
    Guid ContenidoId, string Clave, string Titulo, List<string> Parrafos) : IRequest;
