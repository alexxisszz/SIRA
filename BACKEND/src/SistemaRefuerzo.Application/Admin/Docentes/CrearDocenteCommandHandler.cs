using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public class CrearDocenteCommandHandler(
    IUsuarioRepository usuarioRepository,
    IDocenteRepository docenteRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearDocenteCommand, Guid>
{
    public async Task<Guid> Handle(CrearDocenteCommand request, CancellationToken cancellationToken)
    {
        var usuarioExistente = await usuarioRepository.ObtenerPorCorreoAsync(request.CorreoElectronico, cancellationToken);
        if (usuarioExistente is not null)
            throw new ReglaDeNegocioException("Ya existe un usuario registrado con ese correo electrónico.");

        var usuario = new Usuario(request.CorreoElectronico, passwordHasher.Hashear(request.Contrasena), Rol.Docente);
        var docente = new Docente(usuario.Id, request.Nombres, request.Apellidos);

        usuarioRepository.Agregar(usuario);
        docenteRepository.Agregar(docente);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return docente.Id;
    }
}
