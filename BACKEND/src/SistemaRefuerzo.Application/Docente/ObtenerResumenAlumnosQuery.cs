using MediatR;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerResumenAlumnosQuery(Guid? TemaId = null) : IRequest<List<AlumnoResumenDto>>;
