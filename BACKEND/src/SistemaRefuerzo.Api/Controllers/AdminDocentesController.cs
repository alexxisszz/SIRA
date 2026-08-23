using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Admin.Docentes;

namespace SistemaRefuerzo.Api.Controllers;

public record CrearDocenteRequest(string CorreoElectronico, string Contrasena, string Nombres, string Apellidos);
public record ActualizarDocenteRequest(string Nombres, string Apellidos);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin/docentes")]
public class AdminDocentesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminDocenteDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var docentes = await sender.Send(new ObtenerDocentesAdminQuery(), cancellationToken);
        return Ok(docentes);
    }

    [HttpPost]
    public async Task<ActionResult> Crear(CrearDocenteRequest request, CancellationToken cancellationToken)
    {
        var docenteId = await sender.Send(
            new CrearDocenteCommand(request.CorreoElectronico, request.Contrasena, request.Nombres, request.Apellidos),
            cancellationToken);

        return Ok(new { docenteId });
    }

    [HttpPut("{docenteId:guid}")]
    public async Task<IActionResult> Actualizar(Guid docenteId, ActualizarDocenteRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ActualizarDocenteCommand(docenteId, request.Nombres, request.Apellidos), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{docenteId:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid docenteId, CambiarEstadoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new CambiarEstadoDocenteCommand(docenteId, request.Activo), cancellationToken);
        return NoContent();
    }
}
