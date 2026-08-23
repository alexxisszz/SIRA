using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class PreguntaRepository(AppDbContext dbContext) : IPreguntaRepository
{
    public Task<List<Pregunta>> ObtenerPorTemaAsync(Guid temaId, CancellationToken cancellationToken) =>
        dbContext.Preguntas
            .Include(p => p.Opciones)
            .Where(p => p.TemaId == temaId)
            .ToListAsync(cancellationToken);

    public Task<List<Pregunta>> ObtenerPorTemaYNivelAsync(Guid temaId, NivelDesempeno nivel, CancellationToken cancellationToken) =>
        dbContext.Preguntas
            .Include(p => p.Opciones)
            .Where(p => p.TemaId == temaId && p.NivelDificultad == nivel)
            .ToListAsync(cancellationToken);

    public Task<Pregunta?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Preguntas
            .Include(p => p.Opciones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public void Agregar(Pregunta pregunta) => dbContext.Preguntas.Add(pregunta);

    public void Eliminar(Pregunta pregunta) => dbContext.Preguntas.Remove(pregunta);
}
