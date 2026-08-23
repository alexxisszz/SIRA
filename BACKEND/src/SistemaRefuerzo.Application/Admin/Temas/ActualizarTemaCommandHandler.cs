using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Temas;

public class ActualizarTemaCommandHandler(
    ITemaRepository temaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarTemaCommand>
{
    public async Task Handle(ActualizarTemaCommand request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        tema.ActualizarDatos(request.Nombre, request.Orden);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
