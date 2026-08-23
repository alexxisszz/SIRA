using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class EvaluacionRepository(AppDbContext dbContext) : IEvaluacionRepository
{
    public Task<Evaluacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Evaluaciones
            .Include(e => e.Respuestas)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public void Agregar(Evaluacion evaluacion) => dbContext.Evaluaciones.Add(evaluacion);
}