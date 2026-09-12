using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Admin.ContenidoTeorico;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Api.Controllers;

public record CrearContenidoTeoricoRequest(Guid TemaId, TipoContenidoTeorico Tipo, string Clave, string Titulo, List<string> Parrafos);
public record ActualizarContenidoTeoricoRequest(string Clave, string Titulo, List<string> Parrafos);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin")]
public class AdminContenidoTeoricoController(ISender sender) : ControllerBase
{
    [HttpGet("temas/{temaId:guid}/teoria")]
    public async Task<ActionResult<List<AdminContenidoTeoricoDto>>> ObtenerPorTema(Guid temaId, CancellationToken cancellationToken)
    {
        var contenidos = await sender.Send(new ObtenerContenidoTeoricoAdminQuery(temaId), cancellationToken);
        return Ok(contenidos);
    }

    [HttpPost("teoria")]
    public async Task<ActionResult> Crear(CrearContenidoTeoricoRequest request, CancellationToken cancellationToken)
    {
        var contenidoId = await sender.Send(
            new CrearContenidoTeoricoCommand(request.TemaId, request.Tipo, request.Clave, request.Titulo, request.Parrafos),
            cancellationToken);

        return Ok(new { contenidoId });
    }

    [HttpPut("teoria/{contenidoId:guid}")]
    public async Task<IActionResult> Actualizar(Guid contenidoId, ActualizarContenidoTeoricoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarContenidoTeoricoCommand(contenidoId, request.Clave, request.Titulo, request.Parrafos),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("teoria/{contenidoId:guid}")]
    public async Task<IActionResult> Eliminar(Guid contenidoId, CancellationToken cancellationToken)
    {
        await sender.Send(new EliminarContenidoTeoricoCommand(contenidoId), cancellationToken);
        return NoContent();
    }
}
