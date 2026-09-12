using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Ejercicios;
using SistemaRefuerzo.Application.Evaluaciones;

namespace SistemaRefuerzo.Api.Controllers;

public record RegistrarIntentoRequest(Guid OpcionSeleccionadaId);

[ApiController]
[Authorize(Roles = "Alumno")]
[Route("api/ejercicios")]
public class EjerciciosController(ISender sender) : ControllerBase
{
    [HttpGet("{preguntaId:guid}")]
    public async Task<ActionResult<PreguntaDto>> Obtener(Guid preguntaId, CancellationToken cancellationToken)
    {
        var ejercicio = await sender.Send(new ObtenerEjercicioQuery(preguntaId), cancellationToken);
        return Ok(ejercicio);
    }

    [HttpPost("{preguntaId:guid}/intentos")]
    public async Task<ActionResult<IntentoResultadoDto>> RegistrarIntento(
        Guid preguntaId, RegistrarIntentoRequest request, CancellationToken cancellationToken)
    {
        var resultado = await sender.Send(
            new RegistrarIntentoCommand(ObtenerUsuarioId(), preguntaId, request.OpcionSeleccionadaId),
            cancellationToken);

        return Ok(resultado);
    }

    private Guid ObtenerUsuarioId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
