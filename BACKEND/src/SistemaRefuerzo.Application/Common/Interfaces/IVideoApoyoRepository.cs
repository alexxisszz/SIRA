using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IVideoApoyoRepository
{
    Task<List<VideoApoyo>> ObtenerPorTemaAsync(Guid temaId, CancellationToken cancellationToken);
    Task<VideoApoyo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(VideoApoyo video);
    void Eliminar(VideoApoyo video);
}
