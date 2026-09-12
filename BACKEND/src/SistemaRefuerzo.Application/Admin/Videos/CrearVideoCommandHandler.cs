using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Videos;

public class CrearVideoCommandHandler(
    ITemaRepository temaRepository,
    IVideoApoyoRepository videoApoyoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearVideoCommand, Guid>
{
    public async Task<Guid> Handle(CrearVideoCommand request, CancellationToken cancellationToken)
    {
        ValidadorDeUrlVideo.Validar(request.Url);

        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var video = new VideoApoyo(tema.Id, request.Titulo, request.Url);

        videoApoyoRepository.Agregar(video);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return video.Id;
    }
}
