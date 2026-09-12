using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin;

public record AdminContenidoTeoricoDto(
    Guid Id, Guid TemaId, TipoContenidoTeorico Tipo, string Clave, string Titulo, List<string> Parrafos);
