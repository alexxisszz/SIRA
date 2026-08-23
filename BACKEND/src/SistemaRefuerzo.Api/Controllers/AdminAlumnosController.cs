using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Admin.Alumnos;

namespace SistemaRefuerzo.Api.Controllers;

public record CrearAlumnoRequest(string CorreoElectronico, string Contrasena, string Nombres, string Apellidos, string Grado);
public record ActualizarAlumnoRequest(string Nombres, string Apellidos, string Grado);
public record CambiarEstadoRequest(bool Activo);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin/alumnos")]
public class AdminAlumnosController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminAlumnoDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var alumnos = await sender.Send(new ObtenerAlumnosAdminQuery(), cancellationToken);
        return Ok(alumnos);
    }

    [HttpPost]
    public async Task<ActionResult> Crear(CrearAlumnoRequest request, CancellationToken cancellationToken)
    {
        var alumnoId = await sender.Send(
            new CrearAlumnoCommand(request.CorreoElectronico, request.Contrasena, request.Nombres, request.Apellidos, request.Grado),
            cancellationToken);

        return Ok(new { alumnoId });
    }

    [HttpPut("{alumnoId:guid}")]
    public async Task<IActionResult> Actualizar(Guid alumnoId, ActualizarAlumnoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ActualizarAlumnoCommand(alumnoId, request.Nombres, request.Apellidos, request.Grado), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{alumnoId:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid alumnoId, CambiarEstadoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new CambiarEstadoAlumnoCommand(alumnoId, request.Activo), cancellationToken);
        return NoContent();
    }
}
