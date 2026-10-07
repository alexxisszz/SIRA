using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class AlumnoRepository(AppDbContext dbContext) : IAlumnoRepository
{
    public Task<Alumno?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Alumnos.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<List<Alumno>> ListarAsync(CancellationToken cancellationToken) =>
        dbContext.Alumnos.AsNoTracking()
            .Where(a => dbContext.Usuarios.Any(u => u.Id == a.UsuarioId && u.Activo))
            .ToListAsync(cancellationToken);

    public void Agregar(Alumno alumno) => dbContext.Alumnos.Add(alumno);
}
