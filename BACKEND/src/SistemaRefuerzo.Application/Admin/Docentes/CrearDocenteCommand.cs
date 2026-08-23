using MediatR;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public record CrearDocenteCommand(
    string CorreoElectronico,
    string Contrasena,
    string Nombres,
    string Apellidos) : IRequest<Guid>;
