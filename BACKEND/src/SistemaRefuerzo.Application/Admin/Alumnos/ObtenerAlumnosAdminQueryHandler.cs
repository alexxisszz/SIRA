using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public class ObtenerAlumnosAdminQueryHandler(IAdminQueryRepository adminQueryRepository)
    : IRequestHandler<ObtenerAlumnosAdminQuery, List<AdminAlumnoDto>>
{
    public Task<List<AdminAlumnoDto>> Handle(ObtenerAlumnosAdminQuery request, CancellationToken cancellationToken) =>
        adminQueryRepository.ObtenerAlumnosAsync(cancellationToken);
}
