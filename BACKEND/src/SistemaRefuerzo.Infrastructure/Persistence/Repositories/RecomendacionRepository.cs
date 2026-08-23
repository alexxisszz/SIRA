using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class RecomendacionRepository(AppDbContext dbContext) : IRecomendacionRepository
{
    public Task<Recomendacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Recomendaciones
            .Include(r => r.EjerciciosRecomendados)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public void Agregar(Recomendacion recomendacion) => dbContext.Recomendaciones.Add(recomendacion);
}