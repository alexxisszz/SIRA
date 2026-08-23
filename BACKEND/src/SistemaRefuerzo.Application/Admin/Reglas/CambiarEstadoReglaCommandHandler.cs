using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Reglas;

public class CambiarEstadoReglaCommandHandler(
    IReglaRepository reglaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CambiarEstadoReglaCommand>
{
    public async Task Handle(CambiarEstadoReglaCommand request, CancellationToken cancellationToken)
    {
        var regla = await reglaRepository.ObtenerPorIdAsync(request.ReglaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Regla), request.ReglaId);

        if (request.Activa)
            regla.Activar();
        else
            regla.Desactivar();

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
