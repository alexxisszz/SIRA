using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Evaluaciones;

namespace SistemaRefuerzo.Api.Controllers;

public record IniciarEvaluacionRequest(Guid TemaId);
public record RegistrarRespuestaRequest(Guid PreguntaId, Guid OpcionSeleccionadaId);

[ApiController]
[Authorize(Roles = "Alumno")]
[Route("api/evaluaciones")]
public class EvaluacionesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Iniciar(IniciarEvaluacionRequest request, CancellationToken cancellationToken)
    {
        var evaluacionId = await sender.Send(new IniciarEvaluacionCommand(request.TemaId, ObtenerUsuarioId()), cancellationToken);
        return Ok(new { evaluacionId });
    }

    [HttpPost("{evaluacionId:guid}/respuestas")]
    public async Task<IActionResult> RegistrarRespuesta(
        Guid evaluacionId,
        RegistrarRespuestaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new RegistrarRespuestaCommand(evaluacionId, request.PreguntaId, request.OpcionSeleccionadaId),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{evaluacionId:guid}/finalizar")]
    public async Task<ActionResult> Finalizar(Guid evaluacionId, CancellationToken cancellationToken)
    {
        var recomendacionId = await sender.Send(new FinalizarEvaluacionCommand(evaluacionId), cancellationToken);
        return Ok(new { recomendacionId });
    }

    private Guid ObtenerUsuarioId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}