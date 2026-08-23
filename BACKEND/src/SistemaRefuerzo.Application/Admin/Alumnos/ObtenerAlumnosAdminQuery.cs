using MediatR;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public record ObtenerAlumnosAdminQuery : IRequest<List<AdminAlumnoDto>>;
