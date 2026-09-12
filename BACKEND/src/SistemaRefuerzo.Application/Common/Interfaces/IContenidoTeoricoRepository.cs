using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IContenidoTeoricoRepository
{
    Task<List<ContenidoTeorico>> ObtenerPorTemaAsync(Guid temaId, CancellationToken cancellationToken);
    Task<ContenidoTeorico?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(ContenidoTeorico contenido);
    void Eliminar(ContenidoTeorico contenido);
}
