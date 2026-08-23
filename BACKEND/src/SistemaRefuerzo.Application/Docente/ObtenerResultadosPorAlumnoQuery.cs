using MediatR;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerResultadosPorAlumnoQuery(Guid AlumnoId) : IRequest<List<ResultadoHistoricoDto>>;
