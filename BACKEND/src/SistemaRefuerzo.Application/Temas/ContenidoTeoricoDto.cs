using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Temas;

public record ContenidoTeoricoDto(Guid Id, TipoContenidoTeorico Tipo, string Clave, string Titulo, List<string> Parrafos);
