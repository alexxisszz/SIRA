using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public class ObtenerContenidoTeoricoAdminQueryHandler(IContenidoTeoricoRepository contenidoTeoricoRepository)
    : IRequestHandler<ObtenerContenidoTeoricoAdminQuery, List<AdminContenidoTeoricoDto>>
{
    public async Task<List<AdminContenidoTeoricoDto>> Handle(ObtenerContenidoTeoricoAdminQuery request, CancellationToken cancellationToken)
    {
        var contenidos = await contenidoTeoricoRepository.ObtenerPorTemaAsync(request.TemaId, cancellationToken);

        return contenidos
            .Select(c => new AdminContenidoTeoricoDto(c.Id, c.TemaId, c.Tipo, c.Clave, c.Titulo, c.Parrafos.ToList()))
            .ToList();
    }
}
