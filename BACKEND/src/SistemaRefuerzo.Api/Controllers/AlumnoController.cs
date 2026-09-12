using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Alumnos;
using SistemaRefuerzo.Application.Reportes.Docente;

namespace SistemaRefuerzo.Api.Controllers;

[ApiController]
[Authorize(Roles = "Alumno")]
[Route("api/alumno")]
public class AlumnoController(ISender sender) : ControllerBase
{
    [HttpGet("perfil")]
    public async Task<ActionResult<PerfilAlumnoDto>> ObtenerMiPerfil(CancellationToken cancellationToken)
    {
        var perfil = await sender.Send(new ObtenerMiPerfilQuery(ObtenerUsuarioId()), cancellationToken);
        return Ok(perfil);
    }

    [HttpGet("historial")]
    public async Task<ActionResult<List<ResultadoHistoricoDto>>> ObtenerMiHistorial(CancellationToken cancellationToken)
    {
        var historial = await sender.Send(new ObtenerMiHistorialQuery(ObtenerUsuarioId()), cancellationToken);
        return Ok(historial);
    }

    [HttpGet("resumen-progreso")]
    public async Task<ActionResult<ResumenProgresoDto>> ObtenerResumenProgreso(CancellationToken cancellationToken)
    {
        var resumen = await sender.Send(new ObtenerResumenProgresoQuery(ObtenerUsuarioId()), cancellationToken);
        return Ok(resumen);
    }

    private Guid ObtenerUsuarioId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
