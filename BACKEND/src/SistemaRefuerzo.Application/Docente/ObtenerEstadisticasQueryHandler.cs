using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public class ObtenerEstadisticasQueryHandler(IDocenteQueryRepository docenteQueryRepository)
    : IRequestHandler<ObtenerEstadisticasQuery, EstadisticasDto>
{
    public Task<EstadisticasDto> Handle(ObtenerEstadisticasQuery request, CancellationToken cancellationToken) =>
        docenteQueryRepository.ObtenerEstadisticasAsync(cancellationToken);
}
