using MediatR;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public record ActualizarAlumnoCommand(Guid AlumnoId, string Nombres, string Apellidos, string Grado) : IRequest;
