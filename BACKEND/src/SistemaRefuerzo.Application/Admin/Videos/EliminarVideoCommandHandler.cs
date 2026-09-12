using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Videos;

public class EliminarVideoCommandHandler(
    IVideoApoyoRepository videoApoyoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<EliminarVideoCommand>
{
    public async Task Handle(EliminarVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await videoApoyoRepository.ObtenerPorIdAsync(request.VideoId, cancellationToken)
            ?? throw new NotFoundException(nameof(VideoApoyo), request.VideoId);

        videoApoyoRepository.Eliminar(video);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
