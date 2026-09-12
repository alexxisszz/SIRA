using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Ejercicios;
using SistemaRefuerzo.Application.Evaluaciones;
using SistemaRefuerzo.Application.Temas;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/temas")]
public class TemasController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TemaDto>>> ObtenerTemas(CancellationToken cancellationToken)
    {
        var temas = await sender.Send(new ObtenerTemasQuery(), cancellationToken);
        return Ok(temas);
    }

    [HttpGet("{temaId:guid}/preguntas")]
    public async Task<ActionResult<List<PreguntaDto>>> ObtenerPreguntas(
        Guid temaId,
        [FromQuery] NivelDesempeno? nivel,
        [FromQuery] string? subtema,
        [FromQuery] string? excluirSubtemas,
        CancellationToken cancellationToken)
    {
        var listaExcluir = excluirSubtemas?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        var preguntas = await sender.Send(
            new ObtenerPreguntasPorTemaQuery(temaId, nivel, subtema, listaExcluir), cancellationToken);
        return Ok(preguntas);
    }

    [HttpGet("{temaId:guid}/videos")]
    public async Task<ActionResult<List<VideoApoyoDto>>> ObtenerVideos(Guid temaId, CancellationToken cancellationToken)
    {
        var videos = await sender.Send(new ObtenerVideosPorTemaQuery(temaId), cancellationToken);
        return Ok(videos);
    }

    [HttpGet("{temaId:guid}/teoria")]
    public async Task<ActionResult<List<ContenidoTeoricoDto>>> ObtenerTeoria(Guid temaId, CancellationToken cancellationToken)
    {
        var contenido = await sender.Send(new ObtenerContenidoTeoricoPorTemaQuery(temaId), cancellationToken);
        return Ok(contenido);
    }

    [HttpGet("{temaId:guid}/estado")]
    [Authorize(Roles = "Alumno")]
    public async Task<ActionResult<EstadoTemaDto>> ObtenerEstado(Guid temaId, CancellationToken cancellationToken)
    {
        var estado = await sender.Send(new ObtenerEstadoTemaQuery(temaId, ObtenerUsuarioId()), cancellationToken);
        return Ok(estado);
    }

    [HttpGet("{temaId:guid}/progreso-practica")]
    [Authorize(Roles = "Alumno")]
    public async Task<ActionResult<List<ProgresoSubtemaDto>>> ObtenerProgresoPractica(Guid temaId, CancellationToken cancellationToken)
    {
        var progreso = await sender.Send(new ObtenerProgresoPracticaQuery(temaId, ObtenerUsuarioId()), cancellationToken);
        return Ok(progreso);
    }

    private Guid ObtenerUsuarioId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
