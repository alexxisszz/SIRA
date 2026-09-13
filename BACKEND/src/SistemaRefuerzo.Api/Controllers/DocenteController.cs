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
    public async Task<ActionResult<List<AlumnoResumenDto>>> ObtenerAlumnos(
        [FromQuery] Guid? temaId, CancellationToken cancellationToken)
    {
        var alumnos = await sender.Send(new ObtenerResumenAlumnosQuery(temaId), cancellationToken);
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

    [HttpGet("resumen-grupo")]
    public async Task<ActionResult<ResumenGrupoDto>> ObtenerResumenGrupo(
        [FromQuery] Guid? temaId, CancellationToken cancellationToken)
    {
        var resumen = await sender.Send(new ObtenerResumenGrupoQuery(temaId), cancellationToken);
        return Ok(resumen);
    }

    [HttpGet("alumnos/{alumnoId:guid}/perfil")]
    public async Task<ActionResult<PerfilRendimientoAlumnoDto>> ObtenerPerfilAlumno(Guid alumnoId, CancellationToken cancellationToken)
    {
        var perfil = await sender.Send(new ObtenerPerfilAlumnoQuery(alumnoId), cancellationToken);
        return Ok(perfil);
    }
}
