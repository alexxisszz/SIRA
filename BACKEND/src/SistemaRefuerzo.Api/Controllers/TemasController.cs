using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Evaluaciones;
using SistemaRefuerzo.Application.Temas;

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
    public async Task<ActionResult<List<PreguntaDto>>> ObtenerPreguntas(Guid temaId, CancellationToken cancellationToken)
    {
        var preguntas = await sender.Send(new ObtenerPreguntasPorTemaQuery(temaId), cancellationToken);
        return Ok(preguntas);
    }
}