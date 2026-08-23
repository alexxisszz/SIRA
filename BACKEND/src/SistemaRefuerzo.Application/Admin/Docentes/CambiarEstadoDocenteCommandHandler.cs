using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public class CambiarEstadoDocenteCommandHandler(
    IDocenteRepository docenteRepository,
    IUsuarioRepository usuarioRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CambiarEstadoDocenteCommand>
{
    public async Task Handle(CambiarEstadoDocenteCommand request, CancellationToken cancellationToken)
    {
        var docente = await docenteRepository.ObtenerPorIdAsync(request.DocenteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Docente), request.DocenteId);

        var usuario = await usuarioRepository.ObtenerPorIdAsync(docente.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Usuario), docente.UsuarioId);

        if (request.Activo)
            usuario.Activar();
        else
            usuario.Desactivar();

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
