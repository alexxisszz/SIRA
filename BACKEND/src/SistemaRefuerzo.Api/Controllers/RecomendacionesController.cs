using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Recomendaciones;

namespace SistemaRefuerzo.Api.Controllers;

[ApiController]
[Authorize(Roles = "Alumno")]
[Route("api/recomendaciones")]
public class RecomendacionesController(ISender sender) : ControllerBase
{
    [HttpGet("{recomendacionId:guid}")]
    public async Task<ActionResult<RecomendacionDto>> ObtenerPorId(Guid recomendacionId, CancellationToken cancellationToken)
    {
        var recomendacion = await sender.Send(new ObtenerRecomendacionQuery(recomendacionId, ObtenerUsuarioId()), cancellationToken);
        return Ok(recomendacion);
    }

    private Guid ObtenerUsuarioId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}