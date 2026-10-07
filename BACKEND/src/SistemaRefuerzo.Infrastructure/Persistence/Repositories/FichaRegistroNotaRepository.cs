using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class FichaRegistroNotaRepository(AppDbContext dbContext) : IFichaRegistroNotaRepository
{
    public Task<FichaRegistroNota?> ObtenerAsync(
        Guid alumnoId, Guid temaId, TipoEvaluacion tipo, CancellationToken cancellationToken) =>
        dbContext.FichasRegistroNotas.FirstOrDefaultAsync(
            f => f.AlumnoId == alumnoId && f.TemaId == temaId && f.TipoEvaluacion == tipo, cancellationToken);

    /// <summary>
    /// Upsert: si la ficha es nueva (no rastreada por el contexto) se agrega; si se obtuvo con
    /// <see cref="ObtenerAsync"/> ya está rastreada y sus cambios se persisten con la unidad de trabajo.
    /// </summary>
    public Task GuardarAsync(FichaRegistroNota ficha, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(ficha).State == EntityState.Detached)
            dbContext.FichasRegistroNotas.Add(ficha);

        return Task.CompletedTask;
    }

    public Task<List<FichaRegistroNota>> ListarPorTemaAsync(Guid temaId, TipoEvaluacion tipo, CancellationToken cancellationToken) =>
        dbContext.FichasRegistroNotas.AsNoTracking()
            .Where(f => f.TemaId == temaId && f.TipoEvaluacion == tipo)
            .ToListAsync(cancellationToken);
}
