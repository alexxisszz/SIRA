using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public class ObtenerDocentesAdminQueryHandler(IAdminQueryRepository adminQueryRepository)
    : IRequestHandler<ObtenerDocentesAdminQuery, List<AdminDocenteDto>>
{
    public Task<List<AdminDocenteDto>> Handle(ObtenerDocentesAdminQuery request, CancellationToken cancellationToken) =>
        adminQueryRepository.ObtenerDocentesAsync(cancellationToken);
}
