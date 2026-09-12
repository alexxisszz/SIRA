using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Videos;

public class ActualizarVideoCommandHandler(
    IVideoApoyoRepository videoApoyoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarVideoCommand>
{
    public async Task Handle(ActualizarVideoCommand request, CancellationToken cancellationToken)
    {
        ValidadorDeUrlVideo.Validar(request.Url);

        var video = await videoApoyoRepository.ObtenerPorIdAsync(request.VideoId, cancellationToken)
            ?? throw new NotFoundException(nameof(VideoApoyo), request.VideoId);

        video.ActualizarContenido(request.Titulo, request.Url);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
