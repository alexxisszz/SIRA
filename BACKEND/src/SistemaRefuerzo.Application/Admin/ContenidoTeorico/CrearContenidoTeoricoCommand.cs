using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public record CrearContenidoTeoricoCommand(
    Guid TemaId, TipoContenidoTeorico Tipo, string Clave, string Titulo, List<string> Parrafos) : IRequest<Guid>;
