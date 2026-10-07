using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Reportes.Docente;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Api.Controllers;

public record RegistrarNotasDocenteRequest(
    Guid AlumnoId,
    Guid TemaId,
    TipoEvaluacion TipoEvaluacion,
    decimal D2I1,
    decimal D2I2,
    decimal D2I3,
    decimal D3I1,
    decimal D3I2,
    decimal D3I3);

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

    [HttpGet("fichas-notas")]
    public async Task<ActionResult<FichaRegistroNotasDto>> ObtenerFichaRegistroNotas(
        [FromQuery] Guid temaId, [FromQuery] TipoEvaluacion tipo, CancellationToken cancellationToken)
    {
        var ficha = await sender.Send(new ObtenerFichaRegistroNotasQuery(temaId, tipo), cancellationToken);
        return Ok(ficha);
    }

    [HttpPost("fichas-notas/notas-manuales")]
    public async Task<IActionResult> RegistrarNotasManuales(RegistrarNotasDocenteRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new RegistrarNotasDocenteCommand(
                request.AlumnoId, request.TemaId, request.TipoEvaluacion,
                request.D2I1, request.D2I2, request.D2I3,
                request.D3I1, request.D3I2, request.D3I3),
            cancellationToken);

        return NoContent();
    }
}
