using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Reportes.Docente;

namespace SistemaRefuerzo.Api.Controllers;

[ApiController]
[Authorize(Roles = "Docente,Administrador")]
[Route("api/docente")]
public class DocenteController(ISender sender) : ControllerBase
{
    [HttpGet("alumnos")]
    public async Task<ActionResult<List<AlumnoResumenDto>>> ObtenerAlumnos(CancellationToken cancellationToken)
    {
        var alumnos = await sender.Send(new ObtenerResumenAlumnosQuery(), cancellationToken);
        return Ok(alumnos);
    }

    [HttpGet("alumnos/{alumnoId:guid}/resultados")]
    public async Task<ActionResult<List<ResultadoHistoricoDto>>> ObtenerResultados(Guid alumnoId, CancellationToken cancellationToken)
    {
        var resultados = await sender.Send(new ObtenerResultadosPorAlumnoQuery(alumnoId), cancellationToken);
        return Ok(resultados);
    }

    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasDto>> ObtenerEstadisticas(CancellationToken cancellationToken)
    {
        var estadisticas = await sender.Send(new ObtenerEstadisticasQuery(), cancellationToken);
        return Ok(estadisticas);
    }
}
