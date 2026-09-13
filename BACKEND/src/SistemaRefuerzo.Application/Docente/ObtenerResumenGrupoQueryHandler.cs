using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public class ObtenerResumenGrupoQueryHandler(IDocenteQueryRepository docenteQueryRepository)
    : IRequestHandler<ObtenerResumenGrupoQuery, ResumenGrupoDto>
{
    public Task<ResumenGrupoDto> Handle(ObtenerResumenGrupoQuery request, CancellationToken cancellationToken) =>
        docenteQueryRepository.ObtenerResumenGrupoAsync(request.TemaId, cancellationToken);
}
