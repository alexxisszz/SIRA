using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.Videos;

public class ObtenerVideosAdminQueryHandler(IVideoApoyoRepository videoApoyoRepository)
    : IRequestHandler<ObtenerVideosAdminQuery, List<AdminVideoApoyoDto>>
{
    public async Task<List<AdminVideoApoyoDto>> Handle(ObtenerVideosAdminQuery request, CancellationToken cancellationToken)
    {
        var videos = await videoApoyoRepository.ObtenerPorTemaAsync(request.TemaId, cancellationToken);

        return videos
            .Select(v => new AdminVideoApoyoDto(v.Id, v.TemaId, v.Titulo, v.Url))
            .ToList();
    }
}
