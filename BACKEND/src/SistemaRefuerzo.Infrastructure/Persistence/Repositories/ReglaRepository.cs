using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class ReglaRepository(AppDbContext dbContext) : IReglaRepository
{
    public Task<List<Regla>> ObtenerActivasAsync(CancellationToken cancellationToken) =>
        dbContext.Reglas.AsNoTracking().Where(r => r.Activa).ToListAsync(cancellationToken);

    public Task<List<Regla>> ObtenerTodasAsync(CancellationToken cancellationToken) =>
        dbContext.Reglas.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Regla?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Reglas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
}
