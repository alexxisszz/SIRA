using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Reglas;

public class ActualizarReglaCommandHandler(
    IReglaRepository reglaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarReglaCommand>
{
    public async Task Handle(ActualizarReglaCommand request, CancellationToken cancellationToken)
    {
        var regla = await reglaRepository.ObtenerPorIdAsync(request.ReglaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Regla), request.ReglaId);

        regla.ActualizarMetadata(request.Nombre, request.DescripcionCondicion, request.DescripcionConclusion, request.Prioridad);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
