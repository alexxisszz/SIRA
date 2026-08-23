using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Recomendaciones;

namespace SistemaRefuerzo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/recomendaciones")]
public class RecomendacionesController(ISender sender) : ControllerBase
{
    [HttpGet("{recomendacionId:guid}")]
    public async Task<ActionResult<RecomendacionDto>> ObtenerPorId(Guid recomendacionId, CancellationToken cancellationToken)
    {
        var recomendacion = await sender.Send(new ObtenerRecomendacionQuery(recomendacionId), cancellationToken);
        return Ok(recomendacion);
    }
}