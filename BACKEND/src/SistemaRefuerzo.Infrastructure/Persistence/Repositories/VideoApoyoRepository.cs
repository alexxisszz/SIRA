using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class VideoApoyoRepository(AppDbContext dbContext) : IVideoApoyoRepository
{
    public Task<List<VideoApoyo>> ObtenerPorTemaAsync(Guid temaId, CancellationToken cancellationToken) =>
        dbContext.VideosApoyo.Where(v => v.TemaId == temaId).ToListAsync(cancellationToken);

    public Task<VideoApoyo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.VideosApoyo.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public void Agregar(VideoApoyo video) => dbContext.VideosApoyo.Add(video);

    public void Eliminar(VideoApoyo video) => dbContext.VideosApoyo.Remove(video);
}
