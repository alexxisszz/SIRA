using SistemaRefuerzo.Application.Admin;

namespace SistemaRefuerzo.Application.Common.Interfaces;

/// <summary>
/// Consultas de solo lectura para el rol Administrador que combinan Usuario con su
/// perfil de Alumno/Docente (correo, estado activo) — igual que IDocenteQueryRepository,
/// se modela aparte de los repositorios de escritura porque son proyecciones de lectura.
/// </summary>
public interface IAdminQueryRepository
{
    Task<List<AdminAlumnoDto>> ObtenerAlumnosAsync(CancellationToken cancellationToken);
    Task<List<AdminDocenteDto>> ObtenerDocentesAsync(CancellationToken cancellationToken);
}
