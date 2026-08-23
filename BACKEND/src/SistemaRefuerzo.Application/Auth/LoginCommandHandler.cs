using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Auth;

public class LoginCommandHandler(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.ObtenerPorCorreoAsync(request.CorreoElectronico, cancellationToken);

        if (usuario is null || !usuario.Activo || !passwordHasher.Verificar(request.Contrasena, usuario.ContrasenaHash))
            throw new CredencialesInvalidasException();

        var token = jwtTokenGenerator.GenerarToken(usuario);

        return new LoginResult(token, usuario.Id, usuario.Rol);
    }
}