using SistemaRefuerzo.Application.Reportes.Docente;

namespace SistemaRefuerzo.Application.Common.Interfaces;

/// <summary>
/// Consultas de solo lectura para el rol Docente. Se modela aparte de los repositorios
/// de escritura de cada agregado porque los reportes combinan datos de varios agregados
/// (Alumno, Evaluacion, Resultado, Recomendacion) proyectados directamente a DTOs.
/// </summary>
public interface IDocenteQueryRepository
{
    Task<List<AlumnoResumenDto>> ObtenerResumenAlumnosAsync(CancellationToken cancellationToken);
    Task<List<ResultadoHistoricoDto>> ObtenerResultadosPorAlumnoAsync(Guid alumnoId, CancellationToken cancellationToken);
    Task<EstadisticasDto> ObtenerEstadisticasAsync(CancellationToken cancellationToken);
}
