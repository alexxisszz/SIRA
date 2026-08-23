using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public class ObtenerResumenAlumnosQueryHandler(IDocenteQueryRepository docenteQueryRepository)
    : IRequestHandler<ObtenerResumenAlumnosQuery, List<AlumnoResumenDto>>
{
    public Task<List<AlumnoResumenDto>> Handle(ObtenerResumenAlumnosQuery request, CancellationToken cancellationToken) =>
        docenteQueryRepository.ObtenerResumenAlumnosAsync(cancellationToken);
}
