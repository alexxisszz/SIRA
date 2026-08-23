using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRefuerzo.Application.Admin;
using SistemaRefuerzo.Application.Admin.Preguntas;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Api.Controllers;

public record OpcionRequest(string Texto, bool EsCorrecta);
public record CrearPreguntaRequest(Guid TemaId, string Enunciado, NivelDesempeno NivelDificultad, List<OpcionRequest> Opciones);
public record ActualizarPreguntaRequest(string Enunciado, NivelDesempeno NivelDificultad, List<OpcionRequest> Opciones);

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin")]
public class AdminPreguntasController(ISender sender) : ControllerBase
{
    [HttpGet("temas/{temaId:guid}/preguntas")]
    public async Task<ActionResult<List<AdminPreguntaDto>>> ObtenerPorTema(Guid temaId, CancellationToken cancellationToken)
    {
        var preguntas = await sender.Send(new ObtenerPreguntasAdminQuery(temaId), cancellationToken);
        return Ok(preguntas);
    }

    [HttpPost("preguntas")]
    public async Task<ActionResult> Crear(CrearPreguntaRequest request, CancellationToken cancellationToken)
    {
        var preguntaId = await sender.Send(
            new CrearPreguntaCommand(
                request.TemaId,
                request.Enunciado,
                request.NivelDificultad,
                request.Opciones.Select(o => new OpcionInput(o.Texto, o.EsCorrecta)).ToList()),
            cancellationToken);

        return Ok(new { preguntaId });
    }

    [HttpPut("preguntas/{preguntaId:guid}")]
    public async Task<IActionResult> Actualizar(Guid preguntaId, ActualizarPreguntaRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarPreguntaCommand(
                preguntaId,
                request.Enunciado,
                request.NivelDificultad,
                request.Opciones.Select(o => new OpcionInput(o.Texto, o.EsCorrecta)).ToList()),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("preguntas/{preguntaId:guid}")]
    public async Task<IActionResult> Eliminar(Guid preguntaId, CancellationToken cancellationToken)
    {
        await sender.Send(new EliminarPreguntaCommand(preguntaId), cancellationToken);
        return NoContent();
    }
}
