using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.ContenidoTeorico;

public class CrearContenidoTeoricoCommandHandler(
    ITemaRepository temaRepository,
    IContenidoTeoricoRepository contenidoTeoricoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearContenidoTeoricoCommand, Guid>
{
    public async Task<Guid> Handle(CrearContenidoTeoricoCommand request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var contenido = new Domain.Entities.ContenidoTeorico(tema.Id, request.Tipo, request.Clave, request.Titulo, request.Parrafos);

        contenidoTeoricoRepository.Agregar(contenido);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return contenido.Id;
    }
}
