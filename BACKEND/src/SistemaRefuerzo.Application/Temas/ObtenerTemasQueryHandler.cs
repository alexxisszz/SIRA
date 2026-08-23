using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Temas;

public class ObtenerTemasQueryHandler(ITemaRepository temaRepository) : IRequestHandler<ObtenerTemasQuery, List<TemaDto>>
{
    public async Task<List<TemaDto>> Handle(ObtenerTemasQuery request, CancellationToken cancellationToken)
    {
        var temas = await temaRepository.ObtenerTodosAsync(cancellationToken);

        return temas
            .OrderBy(t => t.Orden)
            .Select(t => new TemaDto(t.Id, t.Nombre, t.Orden))
            .ToList();
    }
}