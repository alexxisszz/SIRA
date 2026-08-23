using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.Reglas;

public class ObtenerReglasAdminQueryHandler(IReglaRepository reglaRepository)
    : IRequestHandler<ObtenerReglasAdminQuery, List<AdminReglaDto>>
{
    public async Task<List<AdminReglaDto>> Handle(ObtenerReglasAdminQuery request, CancellationToken cancellationToken)
    {
        var reglas = await reglaRepository.ObtenerTodasAsync(cancellationToken);

        return reglas
            .OrderByDescending(r => r.Prioridad)
            .Select(r => new AdminReglaDto(
                r.Id,
                r.Nombre,
                r.NombreClaseRegla,
                r.DescripcionCondicion,
                r.DescripcionConclusion,
                r.Prioridad,
                r.Activa))
            .ToList();
    }
}
