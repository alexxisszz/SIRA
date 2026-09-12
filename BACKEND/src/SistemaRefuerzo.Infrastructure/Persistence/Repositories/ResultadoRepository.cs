using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class ResultadoRepository(AppDbContext dbContext) : IResultadoRepository
{
    public Task<Resultado?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Resultados.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Resultado?> ObtenerPorEvaluacionIdAsync(Guid evaluacionId, CancellationToken cancellationToken) =>
        dbContext.Resultados.AsNoTracking().FirstOrDefaultAsync(r => r.EvaluacionId == evaluacionId, cancellationToken);

    public void Agregar(Resultado resultado) => dbContext.Resultados.Add(resultado);
}
