using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class AdminQueryRepository(AppDbContext dbContext) : IAdminQueryRepository
{
    public async Task<List<AdminAlumnoDto>> ObtenerAlumnosAsync(CancellationToken cancellationToken)
    {
        var alumnos = await dbContext.Alumnos.AsNoTracking().ToListAsync(cancellationToken);
        var usuarios = await dbContext.Usuarios.AsNoTracking().ToListAsync(cancellationToken);

        return alumnos
            .Join(usuarios, a => a.UsuarioId, u => u.Id, (alumno, usuario) => new AdminAlumnoDto(
                alumno.Id,
                usuario.Id,
                usuario.CorreoElectronico,
                usuario.Activo,
                alumno.Nombres,
                alumno.Apellidos,
                alumno.Grado))
            .OrderBy(dto => dto.Apellidos)
            .ThenBy(dto => dto.Nombres)
            .ToList();
    }

    public async Task<List<AdminDocenteDto>> ObtenerDocentesAsync(CancellationToken cancellationToken)
    {
        var docentes = await dbContext.Docentes.AsNoTracking().ToListAsync(cancellationToken);
        var usuarios = await dbContext.Usuarios.AsNoTracking().ToListAsync(cancellationToken);

        return docentes
            .Join(usuarios, d => d.UsuarioId, u => u.Id, (docente, usuario) => new AdminDocenteDto(
                docente.Id,
                usuario.Id,
                usuario.CorreoElectronico,
                usuario.Activo,
                docente.Nombres,
                docente.Apellidos))
            .OrderBy(dto => dto.Apellidos)
            .ThenBy(dto => dto.Nombres)
            .ToList();
    }
}
