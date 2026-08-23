using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class AlumnoRepository(AppDbContext dbContext) : IAlumnoRepository
{
    public Task<Alumno?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Alumnos.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public void Agregar(Alumno alumno) => dbContext.Alumnos.Add(alumno);
}
