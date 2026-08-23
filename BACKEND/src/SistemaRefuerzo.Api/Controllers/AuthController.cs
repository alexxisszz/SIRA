using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Auth;

namespace SistemaRefuerzo.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var resultado = await sender.Send(command, cancellationToken);
        return Ok(resultado);
    }
}