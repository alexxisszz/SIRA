using MediatR;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerPerfilAlumnoQuery(Guid AlumnoId) : IRequest<PerfilRendimientoAlumnoDto>;
