using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Ejercicios;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class PracticaQueryRepository(AppDbContext dbContext) : IPracticaQueryRepository
{
    public async Task<List<ProgresoSubtemaDto>> ObtenerProgresoPorSubtemaAsync(
        Guid alumnoId, Guid temaId, CancellationToken cancellationToken)
    {
        var filas = await dbContext.IntentosEjercicio
            .AsNoTracking()
            .Where(i => i.AlumnoId == alumnoId && i.TemaId == temaId)
            .Select(i => new { i.Subtema, i.EsCorrecta })
            .ToListAsync(cancellationToken);

        return filas
            .GroupBy(f => f.Subtema)
            .Select(g => new ProgresoSubtemaDto(g.Key, g.Count(), g.Count(f => f.EsCorrecta)))
            .OrderBy(dto => dto.Subtema)
            .ToList();
    }
}
