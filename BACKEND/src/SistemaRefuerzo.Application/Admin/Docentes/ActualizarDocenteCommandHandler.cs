using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public class ActualizarDocenteCommandHandler(
    IDocenteRepository docenteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarDocenteCommand>
{
    public async Task Handle(ActualizarDocenteCommand request, CancellationToken cancellationToken)
    {
        var docente = await docenteRepository.ObtenerPorIdAsync(request.DocenteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Docente), request.DocenteId);

        docente.ActualizarDatos(request.Nombres, request.Apellidos);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
