using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Temas;

public class CrearTemaCommandHandler(
    ITemaRepository temaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearTemaCommand, Guid>
{
    public async Task<Guid> Handle(CrearTemaCommand request, CancellationToken cancellationToken)
    {
        var tema = new Tema(request.Nombre, request.Orden);

        temaRepository.Agregar(tema);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return tema.Id;
    }
}
