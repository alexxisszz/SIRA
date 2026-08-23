using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin.Temas;
using SistemaRefuerzo.Application.Temas;

namespace SistemaRefuerzo.Api.Controllers;

public record CrearTemaRequest(string Nombre, int Orden);
public record ActualizarTemaRequest(string Nombre, int Orden);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin/temas")]
public class AdminTemasController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TemaDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var temas = await sender.Send(new ObtenerTemasQuery(), cancellationToken);
        return Ok(temas);
    }

    [HttpPost]
    public async Task<ActionResult> Crear(CrearTemaRequest request, CancellationToken cancellationToken)
    {
        var temaId = await sender.Send(new CrearTemaCommand(request.Nombre, request.Orden), cancellationToken);
        return Ok(new { temaId });
    }

    [HttpPut("{temaId:guid}")]
    public async Task<IActionResult> Actualizar(Guid temaId, ActualizarTemaRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ActualizarTemaCommand(temaId, request.Nombre, request.Orden), cancellationToken);
        return NoContent();
    }
}
