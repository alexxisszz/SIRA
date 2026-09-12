using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class EvaluacionRepository(AppDbContext dbContext) : IEvaluacionRepository
{
    public Task<Evaluacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Evaluaciones
            .Include(e => e.Respuestas)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Evaluacion?> ObtenerUltimaFinalizadaAsync(
        Guid alumnoId, Guid temaId, TipoEvaluacion tipo, NivelDesempeno? nivel, CancellationToken cancellationToken) =>
        dbContext.Evaluaciones
            .Include(e => e.Respuestas)
            .Where(e => e.AlumnoId == alumnoId
                && e.TemaId == temaId
                && e.Tipo == tipo
                && e.NivelEvaluado == nivel
                && e.Estado == EstadoEvaluacion.Finalizada)
            .OrderByDescending(e => e.FechaFin)
            .FirstOrDefaultAsync(cancellationToken);

    public void Agregar(Evaluacion evaluacion) => dbContext.Evaluaciones.Add(evaluacion);
}
