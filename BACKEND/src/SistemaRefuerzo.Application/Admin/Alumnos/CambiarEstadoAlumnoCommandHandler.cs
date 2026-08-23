using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public class CambiarEstadoAlumnoCommandHandler(
    IAlumnoRepository alumnoRepository,
    IUsuarioRepository usuarioRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CambiarEstadoAlumnoCommand>
{
    public async Task Handle(CambiarEstadoAlumnoCommand request, CancellationToken cancellationToken)
    {
        var alumno = await alumnoRepository.ObtenerPorIdAsync(request.AlumnoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.AlumnoId);

        var usuario = await usuarioRepository.ObtenerPorIdAsync(alumno.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Usuario), alumno.UsuarioId);

        if (request.Activo)
            usuario.Activar();
        else
            usuario.Desactivar();

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
