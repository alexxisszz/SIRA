using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Admin.Reglas;

namespace SistemaRefuerzo.Api.Controllers;

public record ActualizarReglaRequest(string Nombre, string DescripcionCondicion, string DescripcionConclusion, int Prioridad);
public record CambiarEstadoReglaRequest(bool Activa);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin/reglas")]
public class AdminReglasController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminReglaDto>>> ObtenerTodas(CancellationToken cancellationToken)
    {
        var reglas = await sender.Send(new ObtenerReglasAdminQuery(), cancellationToken);
        return Ok(reglas);
    }

    [HttpPut("{reglaId:guid}")]
    public async Task<IActionResult> Actualizar(Guid reglaId, ActualizarReglaRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarReglaCommand(reglaId, request.Nombre, request.DescripcionCondicion, request.DescripcionConclusion, request.Prioridad),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{reglaId:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid reglaId, CambiarEstadoReglaRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new CambiarEstadoReglaCommand(reglaId, request.Activa), cancellationToken);
        return NoContent();
    }
}
