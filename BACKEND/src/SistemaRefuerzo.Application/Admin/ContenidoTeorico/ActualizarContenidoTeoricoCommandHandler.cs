using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public class ActualizarContenidoTeoricoCommandHandler(
    IContenidoTeoricoRepository contenidoTeoricoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarContenidoTeoricoCommand>
{
    public async Task Handle(ActualizarContenidoTeoricoCommand request, CancellationToken cancellationToken)
    {
        var contenido = await contenidoTeoricoRepository.ObtenerPorIdAsync(request.ContenidoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.ContenidoTeorico), request.ContenidoId);

        contenido.ActualizarContenido(request.Clave, request.Titulo, request.Parrafos);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
