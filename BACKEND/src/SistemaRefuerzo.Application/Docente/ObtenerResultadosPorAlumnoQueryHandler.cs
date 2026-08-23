using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public class ObtenerResultadosPorAlumnoQueryHandler(IDocenteQueryRepository docenteQueryRepository)
    : IRequestHandler<ObtenerResultadosPorAlumnoQuery, List<ResultadoHistoricoDto>>
{
    public Task<List<ResultadoHistoricoDto>> Handle(ObtenerResultadosPorAlumnoQuery request, CancellationToken cancellationToken) =>
        docenteQueryRepository.ObtenerResultadosPorAlumnoAsync(request.AlumnoId, cancellationToken);
}
