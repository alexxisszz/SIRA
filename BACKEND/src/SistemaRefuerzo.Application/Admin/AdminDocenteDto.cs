namespace SistemaRefuerzo.Application.Admin;

public record AdminDocenteDto(
    Guid DocenteId,
    Guid UsuarioId,
    string CorreoElectronico,
    bool Activo,
    string Nombres,
    string Apellidos);
