using MediatR;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerResumenAlumnosQuery : IRequest<List<AlumnoResumenDto>>;
