using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class ContenidoTeoricoRepository(AppDbContext dbContext) : IContenidoTeoricoRepository
{
    public Task<List<ContenidoTeorico>> ObtenerPorTemaAsync(Guid temaId, CancellationToken cancellationToken) =>
        dbContext.ContenidosTeoricos.Where(c => c.TemaId == temaId).ToListAsync(cancellationToken);

    public Task<ContenidoTeorico?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.ContenidosTeoricos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Agregar(ContenidoTeorico contenido) => dbContext.ContenidosTeoricos.Add(contenido);

    public void Eliminar(ContenidoTeorico contenido) => dbContext.ContenidosTeoricos.Remove(contenido);
}
