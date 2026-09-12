using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Admin.Videos;

namespace SistemaRefuerzo.Api.Controllers;

public record CrearVideoRequest(Guid TemaId, string Titulo, string Url);
public record ActualizarVideoRequest(string Titulo, string Url);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin")]
public class AdminVideosController(ISender sender) : ControllerBase
{
    [HttpGet("temas/{temaId:guid}/videos")]
    public async Task<ActionResult<List<AdminVideoApoyoDto>>> ObtenerPorTema(Guid temaId, CancellationToken cancellationToken)
    {
        var videos = await sender.Send(new ObtenerVideosAdminQuery(temaId), cancellationToken);
        return Ok(videos);
    }

    [HttpPost("videos")]
    public async Task<ActionResult> Crear(CrearVideoRequest request, CancellationToken cancellationToken)
    {
        var videoId = await sender.Send(new CrearVideoCommand(request.TemaId, request.Titulo, request.Url), cancellationToken);
        return Ok(new { videoId });
    }

    [HttpPut("videos/{videoId:guid}")]
    public async Task<IActionResult> Actualizar(Guid videoId, ActualizarVideoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ActualizarVideoCommand(videoId, request.Titulo, request.Url), cancellationToken);
        return NoContent();
    }

    [HttpDelete("videos/{videoId:guid}")]
    public async Task<IActionResult> Eliminar(Guid videoId, CancellationToken cancellationToken)
    {
        await sender.Send(new EliminarVideoCommand(videoId), cancellationToken);
        return NoContent();
    }
}
