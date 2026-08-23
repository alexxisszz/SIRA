using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Auth;

public record LoginCommand(string CorreoElectronico, string Contrasena) : IRequest<LoginResult>;

public record LoginResult(string Token, Guid UsuarioId, Rol Rol);