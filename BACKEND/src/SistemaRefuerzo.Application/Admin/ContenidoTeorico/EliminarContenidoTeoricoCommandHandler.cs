using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public class EliminarContenidoTeoricoCommandHandler(
    IContenidoTeoricoRepository contenidoTeoricoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<EliminarContenidoTeoricoCommand>
{
    public async Task Handle(EliminarContenidoTeoricoCommand request, CancellationToken cancellationToken)
    {
        var contenido = await contenidoTeoricoRepository.ObtenerPorIdAsync(request.ContenidoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.ContenidoTeorico), request.ContenidoId);

        contenidoTeoricoRepository.Eliminar(contenido);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
