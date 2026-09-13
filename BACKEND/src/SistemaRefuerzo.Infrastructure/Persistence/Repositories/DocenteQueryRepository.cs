using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Evaluaciones;
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
    public async Task<List<AlumnoResumenDto>> ObtenerResumenAlumnosAsync(Guid? temaId, CancellationToken cancellationToken)
    {
        var alumnos = await dbContext.Alumnos.AsNoTracking().ToListAsync(cancellationToken);
        var evaluaciones = await ObtenerEvaluacionesFinalizadasAsync(cancellationToken);
        var resultados = await dbContext.Resultados.AsNoTracking().ToListAsync(cancellationToken);
        var recomendaciones = await dbContext.Recomendaciones.AsNoTracking().ToListAsync(cancellationToken);
        var temas = await dbContext.Temas.AsNoTracking().ToListAsync(cancellationToken);
        var intentos = await dbContext.IntentosEjercicio.AsNoTracking().ToListAsync(cancellationToken);

        return alumnos
            .Select(alumno => CalcularRendimientoAlumno(alumno, evaluaciones, resultados, recomendaciones, temas, intentos, temaId))
            .OrderBy(r => r.Apellidos)
            .ThenBy(r => r.Nombres)
            .Select(r => new AlumnoResumenDto(
                r.AlumnoId, r.Nombres, r.Apellidos, alumnos.First(a => a.Id == r.AlumnoId).Grado,
                evaluaciones.Count(e => e.AlumnoId == r.AlumnoId && (temaId == null || e.TemaId == temaId)),
                r.NivelActual, r.UltimaActividad, r.PorcentajeAvance, r.UltimoPuntaje, r.Estado))
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

    public async Task<ResumenGrupoDto> ObtenerResumenGrupoAsync(Guid? temaId, CancellationToken cancellationToken)
    {
        var alumnos = await dbContext.Alumnos.AsNoTracking().ToListAsync(cancellationToken);
        var evaluaciones = await ObtenerEvaluacionesFinalizadasAsync(cancellationToken);
        var resultados = await dbContext.Resultados.AsNoTracking().ToListAsync(cancellationToken);
        var recomendaciones = await dbContext.Recomendaciones.AsNoTracking().ToListAsync(cancellationToken);
        var temas = await dbContext.Temas.AsNoTracking().ToListAsync(cancellationToken);
        var intentos = await dbContext.IntentosEjercicio.AsNoTracking().ToListAsync(cancellationToken);
        var respuestas = await dbContext.RespuestasAlumno.AsNoTracking().ToListAsync(cancellationToken);
        var preguntasPorId = await dbContext.Preguntas.AsNoTracking()
            .ToDictionaryAsync(p => p.Id, p => p.Subtema, cancellationToken);

        var rendimiento = alumnos
            .Select(alumno => CalcularRendimientoAlumno(alumno, evaluaciones, resultados, recomendaciones, temas, intentos, temaId))
            .OrderBy(dto => dto.Apellidos)
            .ThenBy(dto => dto.Nombres)
            .ToList();

        var evaluacionIdsEnAlcance = evaluaciones
            .Where(e => temaId == null || e.TemaId == temaId)
            .Select(e => e.Id)
            .ToHashSet();
        var resultadosEnAlcance = resultados.Where(r => evaluacionIdsEnAlcance.Contains(r.EvaluacionId)).ToList();

        var puntajePromedioGeneral = resultadosEnAlcance.Count > 0 ? resultadosEnAlcance.Average(r => r.Puntaje) : 0;
        var avancePromedio = rendimiento.Count > 0 ? rendimiento.Average(r => r.PorcentajeAvance) : 0;

        var distribucionNiveles = rendimiento
            .Where(r => r.NivelActual is not null)
            .GroupBy(r => r.NivelActual!.Value)
            .Select(grupo => new NivelDistribucionDto(grupo.Key, grupo.Count()))
            .OrderBy(dto => dto.Nivel)
            .ToList();

        var respuestasEnAlcance = respuestas.Where(r => evaluacionIdsEnAlcance.Contains(r.EvaluacionId)).ToList();
        var intentosEnAlcance = temaId is null ? intentos : intentos.Where(i => i.TemaId == temaId).ToList();
        var dificultadesDelGrupo = CalcularDificultadesDelGrupo(respuestasEnAlcance, preguntasPorId, intentosEnAlcance);

        var alertas = rendimiento
            .Where(r => r.Estado == EstadoRequiereApoyo && r.MotivoAlerta is not null)
            .Select(r => new AlertaAlumnoDto(r.AlumnoId, r.Nombres, r.Apellidos, r.NivelActual, r.MotivoAlerta!))
            .ToList();

        return new ResumenGrupoDto(
            alumnos.Count,
            puntajePromedioGeneral,
            avancePromedio,
            alertas.Count,
            distribucionNiveles,
            dificultadesDelGrupo,
            alertas,
            rendimiento.Select(r => r.ADto()).ToList());
    }

    public async Task<PerfilRendimientoAlumnoDto> ObtenerPerfilAlumnoAsync(Guid alumnoId, CancellationToken cancellationToken)
    {
        var alumno = await dbContext.Alumnos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == alumnoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), alumnoId);

        var evaluaciones = await ObtenerEvaluacionesFinalizadasAsync(cancellationToken);
        var resultados = await dbContext.Resultados.AsNoTracking().ToListAsync(cancellationToken);
        var recomendaciones = await dbContext.Recomendaciones.AsNoTracking().ToListAsync(cancellationToken);
        var temas = await dbContext.Temas.AsNoTracking().ToListAsync(cancellationToken);
        var intentos = await dbContext.IntentosEjercicio.AsNoTracking().ToListAsync(cancellationToken);
        var respuestas = await dbContext.RespuestasAlumno.AsNoTracking().ToListAsync(cancellationToken);
        var preguntas = await dbContext.Preguntas.AsNoTracking().ToListAsync(cancellationToken);

        var rendimiento = CalcularRendimientoAlumno(alumno, evaluaciones, resultados, recomendaciones, temas, [.. intentos]);

        var evaluacionesDelAlumno = evaluaciones.Where(e => e.AlumnoId == alumnoId).OrderBy(e => e.FechaFin).ToList();
        var ultimaEvaluacion = evaluacionesDelAlumno.LastOrDefault();
        var temaActual = ultimaEvaluacion is not null ? temas.FirstOrDefault(t => t.Id == ultimaEvaluacion.TemaId) : null;

        var ultimoResultado = ultimaEvaluacion is not null
            ? resultados.FirstOrDefault(r => r.EvaluacionId == ultimaEvaluacion.Id)
            : null;
        var ultimaRecomendacion = ultimoResultado is not null
            ? recomendaciones.FirstOrDefault(r => r.ResultadoId == ultimoResultado.Id)
            : null;

        var rendimientoPorSubtema = temaActual is not null
            ? CalcularRendimientoPorSubtema(alumnoId, temaActual.Id, evaluacionesDelAlumno, respuestas, intentos, preguntas)
            : [];

        var decisionesSistema = ultimaRecomendacion is not null && ultimoResultado is not null
            ? TraductorDeDecisiones.Traducir(
                ultimaRecomendacion.ReglasAplicadas, ultimaRecomendacion.Nivel, ultimoResultado.Puntaje,
                ultimoResultado.FallosConsecutivos, ultimaRecomendacion.TemasPorReforzar, temaActual?.Nombre ?? "")
            : [];

        var evolucion = DeterminarEvolucion(rendimiento.NivelInicial, rendimiento.NivelActual, evaluacionesDelAlumno.Count);

        var evolucionPuntajes = evaluacionesDelAlumno.Count >= 2
            ? evaluacionesDelAlumno
                .Select(e => (Evaluacion: e, Resultado: resultados.FirstOrDefault(r => r.EvaluacionId == e.Id)))
                .Where(par => par.Resultado is not null)
                .Select(par => new PuntoEvolucionDto(
                    par.Evaluacion.FechaFin!.Value,
                    DescribirEvaluacion(par.Evaluacion, temas),
                    par.Resultado!.Puntaje))
                .ToList()
            : [];

        var historial = await ObtenerResultadosPorAlumnoAsync(alumnoId, cancellationToken);

        var (correctas, total) = ContarAciertosGlobales(alumnoId, evaluacionesDelAlumno, respuestas, intentos);

        return new PerfilRendimientoAlumnoDto(
            alumno.Id,
            alumno.Nombres,
            alumno.Apellidos,
            alumno.Grado,
            temaActual?.Nombre,
            rendimiento.UltimaActividad,
            rendimiento.NivelInicial,
            rendimiento.NivelActual,
            evolucion,
            rendimiento.UltimoPuntaje,
            total > 0 ? correctas * 100.0 / total : null,
            evaluacionesDelAlumno.Count,
            rendimientoPorSubtema,
            ultimaRecomendacion?.SubtemasDominados.ToList() ?? [],
            ultimaRecomendacion?.TemasPorReforzar.ToList() ?? [],
            ultimaRecomendacion?.Retroalimentacion,
            decisionesSistema,
            evolucionPuntajes,
            historial);
    }

    private const string EstadoSinEvaluar = "Sin evaluar";
    private const string EstadoRequiereApoyo = "Requiere apoyo";
    private const string EstadoBuenProgreso = "Buen progreso";
    private const string EstadoEnProgreso = "En progreso";
    private const double DiasInactividadAlerta = 7;
    private const double AvancePorcentajeBuenProgreso = 66;

    private sealed record RendimientoInterno(
        Guid AlumnoId,
        string Nombres,
        string Apellidos,
        int? PuntajeEntrada,
        NivelDesempeno? NivelInicial,
        NivelDesempeno? NivelActual,
        double PorcentajeAvance,
        int? UltimoPuntaje,
        DateTime? UltimaActividad,
        string Estado,
        string? MotivoAlerta)
    {
        public RendimientoAlumnoDto ADto() => new(
            AlumnoId, Nombres, Apellidos, PuntajeEntrada, NivelInicial, NivelActual,
            PorcentajeAvance, UltimoPuntaje, UltimaActividad, Estado);
    }

    private RendimientoInterno CalcularRendimientoAlumno(
        Alumno alumno,
        List<Evaluacion> evaluaciones,
        List<Resultado> resultados,
        List<Recomendacion> recomendaciones,
        List<Tema> temas,
        List<IntentoEjercicio> intentos,
        Guid? temaId = null)
    {
        var evaluacionesDelAlumno = evaluaciones
            .Where(e => e.AlumnoId == alumno.Id && (temaId == null || e.TemaId == temaId))
            .ToList();

        NivelDesempeno? NivelDe(Evaluacion evaluacion)
        {
            var resultado = resultados.FirstOrDefault(r => r.EvaluacionId == evaluacion.Id);
            return resultado is null ? null : recomendaciones.FirstOrDefault(r => r.ResultadoId == resultado.Id)?.Nivel;
        }

        int? PuntajeDe(Evaluacion evaluacion) => resultados.FirstOrDefault(r => r.EvaluacionId == evaluacion.Id)?.Puntaje;

        var primerDiagnostico = evaluacionesDelAlumno
            .Where(e => e.Tipo == TipoEvaluacion.Diagnostica)
            .OrderBy(e => e.FechaFin)
            .FirstOrDefault();

        var ultimaEvaluacion = evaluacionesDelAlumno.OrderByDescending(e => e.FechaFin).FirstOrDefault();

        var nivelInicial = primerDiagnostico is not null ? NivelDe(primerDiagnostico) : null;
        var nivelActual = ultimaEvaluacion is not null ? NivelDe(ultimaEvaluacion) : null;
        var puntajeEntrada = primerDiagnostico is not null ? PuntajeDe(primerDiagnostico) : null;
        var ultimoPuntaje = ultimaEvaluacion is not null ? PuntajeDe(ultimaEvaluacion) : null;
        var fallosConsecutivosUltima = ultimaEvaluacion is not null
            ? resultados.FirstOrDefault(r => r.EvaluacionId == ultimaEvaluacion.Id)?.FallosConsecutivos ?? 0
            : 0;

        double porcentajeAvance;
        if (temaId is Guid temaUnico)
            porcentajeAvance = CalcularAvanceTema(alumno.Id, temaUnico, evaluacionesDelAlumno, resultados);
        else
            porcentajeAvance = temas.Count > 0
                ? temas.Average(tema => CalcularAvanceTema(alumno.Id, tema.Id, evaluacionesDelAlumno, resultados))
                : 0;

        var ultimaActividadEvaluacion = evaluacionesDelAlumno.Max(e => (DateTime?)e.FechaFin);
        var ultimaActividadIntento = intentos
            .Where(i => i.AlumnoId == alumno.Id && (temaId == null || i.TemaId == temaId))
            .Select(i => (DateTime?)i.FechaRegistro)
            .DefaultIfEmpty(null)
            .Max();
        var ultimaActividad = new[] { ultimaActividadEvaluacion, ultimaActividadIntento }
            .Where(fecha => fecha is not null)
            .DefaultIfEmpty(null)
            .Max();

        var (estado, motivo) = DeterminarEstado(
            evaluacionesDelAlumno.Count, fallosConsecutivosUltima, ultimoPuntaje, ultimaActividad,
            nivelActual, nivelInicial, porcentajeAvance);

        return new RendimientoInterno(
            alumno.Id, alumno.Nombres, alumno.Apellidos, puntajeEntrada, nivelInicial, nivelActual,
            porcentajeAvance, ultimoPuntaje, ultimaActividad, estado, motivo);
    }

    private static (string Estado, string? Motivo) DeterminarEstado(
        int cantidadEvaluaciones, int fallosConsecutivosUltima, int? ultimoPuntaje, DateTime? ultimaActividad,
        NivelDesempeno? nivelActual, NivelDesempeno? nivelInicial, double porcentajeAvance)
    {
        if (cantidadEvaluaciones == 0)
            return (EstadoSinEvaluar, null);

        if (fallosConsecutivosUltima >= 3)
            return (EstadoRequiereApoyo, $"{fallosConsecutivosUltima} fallos consecutivos en su última evaluación");

        if (ultimoPuntaje is int puntaje && puntaje < ProgresoTemaService.PuntajeAprobacionNivel)
            return (EstadoRequiereApoyo, $"Bajo desempeño en su última evaluación ({puntaje}/100)");

        if (ultimaActividad is DateTime fecha)
        {
            var diasInactivo = (DateTime.UtcNow - fecha).TotalDays;
            if (diasInactivo >= DiasInactividadAlerta)
                return (EstadoRequiereApoyo, $"Sin actividad hace {(int)diasInactivo} días");
        }

        if (nivelActual is not null && nivelActual != NivelDesempeno.Avanzado
            && nivelActual == nivelInicial && porcentajeAvance < 50)
            return (EstadoRequiereApoyo, $"Estancado en nivel {nivelActual}");

        if (porcentajeAvance >= AvancePorcentajeBuenProgreso && ultimoPuntaje >= ProgresoTemaService.PuntajeAprobacionFinal)
            return (EstadoBuenProgreso, null);

        return (EstadoEnProgreso, null);
    }

    /// <summary>
    /// Un tema se considera 100% avanzado cuando el alumno completó sus 5 hitos:
    /// diagnóstico, los tres niveles por dificultad y la prueba final del tema.
    /// </summary>
    private static double CalcularAvanceTema(
        Guid alumnoId, Guid temaId, List<Evaluacion> evaluacionesDelAlumno, List<Resultado> resultados)
    {
        var evaluacionesDelTema = evaluacionesDelAlumno.Where(e => e.TemaId == temaId).ToList();

        bool Aprobada(TipoEvaluacion tipo, NivelDesempeno? nivel, int puntajeMinimo) =>
            evaluacionesDelTema
                .Where(e => e.Tipo == tipo && e.NivelEvaluado == nivel)
                .Select(e => resultados.FirstOrDefault(r => r.EvaluacionId == e.Id))
                .Any(r => r is not null && r.Puntaje >= puntajeMinimo);

        var hitos = new[]
        {
            evaluacionesDelTema.Any(e => e.Tipo == TipoEvaluacion.Diagnostica),
            Aprobada(TipoEvaluacion.PorNivel, NivelDesempeno.Basico, ProgresoTemaService.PuntajeAprobacionNivel),
            Aprobada(TipoEvaluacion.PorNivel, NivelDesempeno.Intermedio, ProgresoTemaService.PuntajeAprobacionNivel),
            Aprobada(TipoEvaluacion.PorNivel, NivelDesempeno.Avanzado, ProgresoTemaService.PuntajeAprobacionNivel),
            Aprobada(TipoEvaluacion.Final, null, ProgresoTemaService.PuntajeAprobacionFinal),
        };

        return hitos.Count(h => h) * 100.0 / hitos.Length;
    }

    private static string DeterminarEvolucion(NivelDesempeno? nivelInicial, NivelDesempeno? nivelActual, int cantidadEvaluaciones)
    {
        if (cantidadEvaluaciones < 2 || nivelInicial is null || nivelActual is null)
            return "Sin datos suficientes";

        if (nivelActual.Value > nivelInicial.Value)
            return "Mejorando";

        if (nivelActual.Value < nivelInicial.Value)
            return "Necesita apoyo";

        return "Estable";
    }

    private static string DescribirEvaluacion(Evaluacion evaluacion, List<Tema> temas)
    {
        var temaNombre = temas.FirstOrDefault(t => t.Id == evaluacion.TemaId)?.Nombre ?? "Tema";
        var tipo = evaluacion.Tipo switch
        {
            TipoEvaluacion.Diagnostica => "Prueba de entrada",
            TipoEvaluacion.Final => "Prueba final",
            _ => evaluacion.NivelEvaluado is NivelDesempeno nivel ? $"Nivel {nivel}" : "Evaluación",
        };
        return $"{tipo} · {temaNombre}";
    }

    private static (int Correctas, int Total) ContarAciertosGlobales(
        Guid alumnoId, List<Evaluacion> evaluacionesDelAlumno, List<RespuestaAlumno> respuestas, List<IntentoEjercicio> intentos)
    {
        var evaluacionIds = evaluacionesDelAlumno.Select(e => e.Id).ToHashSet();
        var respuestasDelAlumno = respuestas.Where(r => evaluacionIds.Contains(r.EvaluacionId)).ToList();
        var intentosDelAlumno = intentos.Where(i => i.AlumnoId == alumnoId).ToList();

        var total = respuestasDelAlumno.Count + intentosDelAlumno.Count;
        var correctas = respuestasDelAlumno.Count(r => r.EsCorrecta) + intentosDelAlumno.Count(i => i.EsCorrecta);
        return (correctas, total);
    }

    /// <summary>
    /// Reutiliza el mismo umbral de dominio del motor de inferencia (60%, ver
    /// <see cref="Domain.InferenceEngine.Reglas.ReglaAnalisisSubtemas"/>) para clasificar cada
    /// subtema del tema actual como Dominado, En progreso o Reforzar, combinando respuestas
    /// de evaluaciones formales y de práctica libre.
    /// </summary>
    private static List<SubtemaRendimientoDto> CalcularRendimientoPorSubtema(
        Guid alumnoId, Guid temaId, List<Evaluacion> evaluacionesDelAlumnoEnTema,
        List<RespuestaAlumno> respuestas, List<IntentoEjercicio> intentos, List<Pregunta> preguntas)
    {
        var evaluacionIdsDelTema = evaluacionesDelAlumnoEnTema.Where(e => e.TemaId == temaId).Select(e => e.Id).ToHashSet();
        var subtemaPorPregunta = preguntas.ToDictionary(p => p.Id, p => p.Subtema);

        var registros = respuestas
            .Where(r => evaluacionIdsDelTema.Contains(r.EvaluacionId) && subtemaPorPregunta.ContainsKey(r.PreguntaId))
            .Select(r => (Subtema: subtemaPorPregunta[r.PreguntaId], r.EsCorrecta))
            .Concat(intentos
                .Where(i => i.AlumnoId == alumnoId && i.TemaId == temaId)
                .Select(i => (i.Subtema, i.EsCorrecta)));

        return registros
            .GroupBy(r => r.Subtema)
            .Select(grupo =>
            {
                var porcentaje = grupo.Count(r => r.EsCorrecta) * 100.0 / grupo.Count();

                string estado;
                if (porcentaje >= 60) estado = "Dominado";
                else if (porcentaje >= 40) estado = "En progreso";
                else estado = "Reforzar";

                return new SubtemaRendimientoDto(grupo.Key, porcentaje, estado);
            })
            .OrderByDescending(dto => dto.PorcentajeAciertos)
            .ToList();
    }

    private const int MinimoIntentosParaDificultad = 5;

    private static List<DificultadGrupoDto> CalcularDificultadesDelGrupo(
        List<RespuestaAlumno> respuestas, Dictionary<Guid, string> preguntasPorId, List<IntentoEjercicio> intentos)
    {
        var registros = respuestas
            .Where(r => preguntasPorId.ContainsKey(r.PreguntaId))
            .Select(r => (Subtema: preguntasPorId[r.PreguntaId], r.EsCorrecta))
            .Concat(intentos.Select(i => (i.Subtema, i.EsCorrecta)));

        return registros
            .GroupBy(r => r.Subtema)
            .Select(grupo => new DificultadGrupoDto(
                grupo.Key,
                grupo.Count(r => !r.EsCorrecta) * 100.0 / grupo.Count(),
                grupo.Count()))
            .Where(dto => dto.TotalIntentos >= MinimoIntentosParaDificultad)
            .OrderByDescending(dto => dto.PorcentajeError)
            .Take(5)
            .ToList();
    }

    private async Task<List<Evaluacion>> ObtenerEvaluacionesFinalizadasAsync(CancellationToken cancellationToken) =>
        await dbContext.Evaluaciones
            .AsNoTracking()
            .Where(e => e.Estado == EstadoEvaluacion.Finalizada)
            .ToListAsync(cancellationToken);
}
