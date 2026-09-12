using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Alumnos;

/// <summary>
/// Progreso real del alumno a través de todos los temas: combina los intentos de
/// práctica libre (<see cref="IntentoEjercicio"/>) con el historial de evaluaciones
/// formales, en vez de estimar cifras (p. ej. "evaluaciones × 10") como hacía el frontend.
/// </summary>
public class ObtenerResumenProgresoQueryHandler(
    IUsuarioRepository usuarioRepository,
    IIntentoEjercicioRepository intentoEjercicioRepository,
    IDocenteQueryRepository docenteQueryRepository) : IRequestHandler<ObtenerResumenProgresoQuery, ResumenProgresoDto>
{
    public async Task<ResumenProgresoDto> Handle(ObtenerResumenProgresoQuery request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var intentos = await intentoEjercicioRepository.ObtenerPorAlumnoAsync(alumno.Id, cancellationToken);
        var historial = await docenteQueryRepository.ObtenerResultadosPorAlumnoAsync(alumno.Id, cancellationToken);

        var fechasActividad = intentos.Select(i => i.FechaRegistro)
            .Concat(historial.Select(h => h.FechaCalculo))
            .ToList();

        return new ResumenProgresoDto(
            EjerciciosIntentados: intentos.Count,
            EjerciciosCorrectos: intentos.Count(i => i.EsCorrecta),
            SubtemasTrabajados: intentos.Select(i => i.Subtema).Distinct().Count(),
            DiasConActividad: fechasActividad.Select(f => f.Date).Distinct().Count(),
            UltimaActividad: fechasActividad.Count > 0 ? fechasActividad.Max() : null);
    }
}
