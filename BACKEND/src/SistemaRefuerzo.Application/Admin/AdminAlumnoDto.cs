namespace SistemaRefuerzo.Application.Admin;

public record AdminAlumnoDto(
    Guid AlumnoId,
    Guid UsuarioId,
    string CorreoElectronico,
    bool Activo,
    string Nombres,
    string Apellidos,
    string Grado);
