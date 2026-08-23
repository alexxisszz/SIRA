using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public class CrearAlumnoCommandHandler(
    IUsuarioRepository usuarioRepository,
    IAlumnoRepository alumnoRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearAlumnoCommand, Guid>
{
    public async Task<Guid> Handle(CrearAlumnoCommand request, CancellationToken cancellationToken)
    {
        var usuarioExistente = await usuarioRepository.ObtenerPorCorreoAsync(request.CorreoElectronico, cancellationToken);
        if (usuarioExistente is not null)
            throw new ReglaDeNegocioException("Ya existe un usuario registrado con ese correo electrónico.");

        var usuario = new Usuario(request.CorreoElectronico, passwordHasher.Hashear(request.Contrasena), Rol.Alumno);
        var alumno = new Alumno(usuario.Id, request.Nombres, request.Apellidos, request.Grado);

        usuarioRepository.Agregar(usuario);
        alumnoRepository.Agregar(alumno);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return alumno.Id;
    }
}
