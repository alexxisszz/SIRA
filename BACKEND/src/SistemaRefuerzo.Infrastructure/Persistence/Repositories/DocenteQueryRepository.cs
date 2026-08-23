using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Reportes.Docente;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementa las consultas de reporte del Docente combinando varios agregados
/// (Alumno, Evaluacion, Resultado, Recomendacion, Tema) mediante joins en memoria.
/// Es deliberadamente distinto de los repositorios de escritura: aquí no importan
/// los límites del agregado, solo proyectar datos de lectura de forma eficiente.
/// </summary>
public class DocenteQueryRepository(AppDbContext dbContext) : IDocenteQueryRepository
{
    public async Task<List<AlumnoResumenDto>> ObtenerResumenAlumnosAsync(CancellationToken cancellationToken)
    {
        var alumnos = await dbContext.Alumnos.AsNoTracking().ToListAsync(cancellationToken);
        var evaluaciones = await ObtenerEvaluacionesFinalizadasAsync(cancellationToken);
        var resultados = await dbContext.Resultados.AsNoTracking().ToListAsync(cancellationToken);
        var recomendaciones = await dbContext.Recomendaciones.AsNoTracking().ToListAsync(cancellationToken);

        return alumnos
            .Select(alumno =>
            {
                var evaluacionesDelAlumno = evaluaciones.Where(e => e.AlumnoId == alumno.Id).ToList();
                var ultimaEvaluacion = evaluacionesDelAlumno
                    .OrderByDescending(e => e.FechaFin)
                    .FirstOrDefault();

                NivelDesempeno? ultimoNivel = null;

                if (ultimaEvaluacion is not null)
                {
                    var resultado = resultados.FirstOrDefault(r => r.EvaluacionId == ultimaEvaluacion.Id);
                    ultimoNivel = resultado is not null
                        ? recomendaciones.FirstOrDefault(r => r.ResultadoId == resultado.Id)?.Nivel
                        : null;
                }

                return new AlumnoResumenDto(
                    alumno.Id,
                    alumno.Nombres,
                    alumno.Apellidos,
                    alumno.Grado,
                    evaluacionesDelAlumno.Count,
                    ultimoNivel,
                    ultimaEvaluacion?.FechaFin);
            })
            .OrderBy(dto => dto.Apellidos)
            .ThenBy(dto => dto.Nombres)
            .ToList();
    }

    public async Task<List<ResultadoHistoricoDto>> ObtenerResultadosPorAlumnoAsync(Guid alumnoId, CancellationToken cancellationToken)
    {
        var evaluaciones = await ObtenerEvaluacionesFinalizadasAsync(cancellationToken);
        var evaluacionesDelAlumno = evaluaciones.Where(e => e.AlumnoId == alumnoId).ToList();

        var resultados = await dbContext.Resultados.AsNoTracking().ToListAsync(cancellationToken);
        var recomendaciones = await dbContext.Recomendaciones.AsNoTracking().ToListAsync(cancellationToken);
        var temas = await dbContext.Temas.AsNoTracking().ToListAsync(cancellationToken);

        var historial = new List<ResultadoHistoricoDto>();

        foreach (var evaluacion in evaluacionesDelAlumno.OrderByDescending(e => e.FechaFin))
        {
            var resultado = resultados.FirstOrDefault(r => r.EvaluacionId == evaluacion.Id);
            var recomendacion = resultado is not null
                ? recomendaciones.FirstOrDefault(r => r.ResultadoId == resultado.Id)
                : null;
            var tema = temas.FirstOrDefault(t => t.Id == evaluacion.TemaId);

            if (resultado is null || recomendacion is null || tema is null)
                continue;

            historial.Add(new ResultadoHistoricoDto(
                evaluacion.Id,
                tema.Nombre,
                resultado.Puntaje,
                resultado.FallosConsecutivos,
                resultado.FechaCalculo,
                recomendacion.Nivel,
                recomendacion.Retroalimentacion));
        }

        return historial;
    }

    public async Task<EstadisticasDto> ObtenerEstadisticasAsync(CancellationToken cancellationToken)
    {
        var evaluaciones = await ObtenerEvaluacionesFinalizadasAsync(cancellationToken);
        var resultados = await dbContext.Resultados.AsNoTracking().ToListAsync(cancellationToken);
        var recomendaciones = await dbContext.Recomendaciones.AsNoTracking().ToListAsync(cancellationToken);
        var temas = await dbContext.Temas.AsNoTracking().ToListAsync(cancellationToken);

        var puntajePromedioGeneral = resultados.Count > 0 ? resultados.Average(r => r.Puntaje) : 0;

        var porTema = temas
            .Select(tema =>
            {
                var evaluacionesDelTema = evaluaciones.Where(e => e.TemaId == tema.Id).ToList();
                var resultadosDelTema = resultados
                    .Where(r => evaluacionesDelTema.Any(e => e.Id == r.EvaluacionId))
                    .ToList();
                var nivelesDelTema = resultadosDelTema
                    .Select(r => recomendaciones.FirstOrDefault(rec => rec.ResultadoId == r.Id)?.Nivel)
                    .Where(nivel => nivel is not null)
                    .Select(nivel => nivel!.Value);

                var distribucionNiveles = nivelesDelTema
                    .GroupBy(nivel => nivel)
                    .Select(grupo => new EstadisticaPorNivelDto(grupo.Key.ToString(), grupo.Count()))
                    .OrderBy(dto => dto.Nivel)
                    .ToList();

                return new EstadisticaPorTemaDto(
                    tema.Nombre,
                    evaluacionesDelTema.Count,
                    resultadosDelTema.Count > 0 ? resultadosDelTema.Average(r => r.Puntaje) : 0,
                    distribucionNiveles);
            })
            .ToList();

        return new EstadisticasDto(evaluaciones.Count, puntajePromedioGeneral, porTema);
    }

    private async Task<List<Evaluacion>> ObtenerEvaluacionesFinalizadasAsync(CancellationToken cancellationToken) =>
        await dbContext.Evaluaciones
            .AsNoTracking()
            .Where(e => e.Estado == EstadoEvaluacion.Finalizada)
            .ToListAsync(cancellationToken);
}
