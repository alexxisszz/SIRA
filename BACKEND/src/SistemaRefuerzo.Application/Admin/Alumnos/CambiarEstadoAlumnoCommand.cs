using MediatR;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public record CambiarEstadoAlumnoCommand(Guid AlumnoId, bool Activo) : IRequest;
