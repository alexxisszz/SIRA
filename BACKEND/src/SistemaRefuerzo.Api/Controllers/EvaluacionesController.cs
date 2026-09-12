using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Evaluaciones;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Api.Controllers;

public record IniciarEvaluacionRequest(Guid TemaId, TipoEvaluacion Tipo, NivelDesempeno? Nivel);
public record RegistrarRespuestaRequest(Guid PreguntaId, Guid OpcionSeleccionadaId);

[ApiController]
[Authorize(Roles = "Alumno")]
[Route("api/evaluaciones")]
public class EvaluacionesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Iniciar(IniciarEvaluacionRequest request, CancellationToken cancellationToken)
    {
        var evaluacionId = await sender.Send(
            new IniciarEvaluacionCommand(request.TemaId, ObtenerUsuarioId(), request.Tipo, request.Nivel),
            cancellationToken);
        return Ok(new { evaluacionId });
    }

    [HttpGet("{evaluacionId:guid}/preguntas")]
    public async Task<ActionResult<List<PreguntaDto>>> ObtenerPreguntas(Guid evaluacionId, CancellationToken cancellationToken)
    {
        var preguntas = await sender.Send(new ObtenerPreguntasDeEvaluacionQuery(evaluacionId, ObtenerUsuarioId()), cancellationToken);
        return Ok(preguntas);
    }

    [HttpPost("{evaluacionId:guid}/respuestas")]
    public async Task<IActionResult> RegistrarRespuesta(
        Guid evaluacionId,
        RegistrarRespuestaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new RegistrarRespuestaCommand(ObtenerUsuarioId(), evaluacionId, request.PreguntaId, request.OpcionSeleccionadaId),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{evaluacionId:guid}/finalizar")]
    public async Task<ActionResult> Finalizar(Guid evaluacionId, CancellationToken cancellationToken)
    {
        var recomendacionId = await sender.Send(new FinalizarEvaluacionCommand(ObtenerUsuarioId(), evaluacionId), cancellationToken);
        return Ok(new { recomendacionId });
    }

    private Guid ObtenerUsuarioId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}