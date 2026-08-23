using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class TemaRepository(AppDbContext dbContext) : ITemaRepository
{
    public Task<List<Tema>> ObtenerTodosAsync(CancellationToken cancellationToken) =>
        dbContext.Temas.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Tema?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Temas.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public void Agregar(Tema tema) => dbContext.Temas.Add(tema);
}
