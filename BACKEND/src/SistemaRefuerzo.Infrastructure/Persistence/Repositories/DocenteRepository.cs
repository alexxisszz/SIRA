using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class DocenteRepository(AppDbContext dbContext) : IDocenteRepository
{
    public Task<Docente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Docentes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Agregar(Docente docente) => dbContext.Docentes.Add(docente);
}
