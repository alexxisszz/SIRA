using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class IntentoEjercicioRepository(AppDbContext dbContext) : IIntentoEjercicioRepository
{
    public void Agregar(IntentoEjercicio intento) => dbContext.IntentosEjercicio.Add(intento);

    public Task<List<IntentoEjercicio>> ObtenerUltimosPorAlumnoYSubtemaAsync(
        Guid alumnoId, string subtema, int cantidad, CancellationToken cancellationToken) =>
        dbContext.IntentosEjercicio
            .AsNoTracking()
            .Where(i => i.AlumnoId == alumnoId && i.Subtema == subtema)
            .OrderByDescending(i => i.FechaRegistro)
            .Take(cantidad)
            .ToListAsync(cancellationToken);

    public Task<List<IntentoEjercicio>> ObtenerPorAlumnoAsync(Guid alumnoId, CancellationToken cancellationToken) =>
        dbContext.IntentosEjercicio
            .AsNoTracking()
            .Where(i => i.AlumnoId == alumnoId)
            .ToListAsync(cancellationToken);
}
