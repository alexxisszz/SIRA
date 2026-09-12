using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Temas;

public class ObtenerContenidoTeoricoPorTemaQueryHandler(IContenidoTeoricoRepository contenidoTeoricoRepository)
    : IRequestHandler<ObtenerContenidoTeoricoPorTemaQuery, List<ContenidoTeoricoDto>>
{
    public async Task<List<ContenidoTeoricoDto>> Handle(ObtenerContenidoTeoricoPorTemaQuery request, CancellationToken cancellationToken)
    {
        var contenidos = await contenidoTeoricoRepository.ObtenerPorTemaAsync(request.TemaId, cancellationToken);

        return contenidos
            .Select(c => new ContenidoTeoricoDto(c.Id, c.Tipo, c.Clave, c.Titulo, c.Parrafos.ToList()))
            .ToList();
    }
}
