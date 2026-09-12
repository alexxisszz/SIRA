using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Temas;

public class ObtenerVideosPorTemaQueryHandler(IVideoApoyoRepository videoApoyoRepository)
    : IRequestHandler<ObtenerVideosPorTemaQuery, List<VideoApoyoDto>>
{
    public async Task<List<VideoApoyoDto>> Handle(ObtenerVideosPorTemaQuery request, CancellationToken cancellationToken)
    {
        var videos = await videoApoyoRepository.ObtenerPorTemaAsync(request.TemaId, cancellationToken);

        return videos.Select(v => new VideoApoyoDto(v.Id, v.Titulo, v.Url)).ToList();
    }
}
