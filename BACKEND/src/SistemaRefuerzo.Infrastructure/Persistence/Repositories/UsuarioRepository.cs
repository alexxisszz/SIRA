using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class UsuarioRepository(AppDbContext dbContext) : IUsuarioRepository
{
    public Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico, CancellationToken cancellationToken) =>
        dbContext.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == correoElectronico, cancellationToken);

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<Alumno?> ObtenerAlumnoPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken) =>
        dbContext.Alumnos.FirstOrDefaultAsync(a => a.UsuarioId == usuarioId, cancellationToken);

    public void Agregar(Usuario usuario) => dbContext.Usuarios.Add(usuario);
}
