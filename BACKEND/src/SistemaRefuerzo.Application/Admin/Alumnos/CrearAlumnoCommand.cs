using MediatR;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public record CrearAlumnoCommand(
    string CorreoElectronico,
    string Contrasena,
    string Nombres,
    string Apellidos,
    string Grado) : IRequest<Guid>;
