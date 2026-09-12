using SistemaRefuerzo.Application.Ejercicios;

namespace SistemaRefuerzo.Application.Common.Interfaces;

/// <summary>
/// Consulta de solo lectura que combina IntentoEjercicio y Pregunta (por Subtema) para
/// reportar el progreso de práctica libre del alumno, sin acoplarse a los repositorios
/// de escritura de cada agregado (mismo criterio que <see cref="IDocenteQueryRepository"/>).
/// </summary>
public interface IPracticaQueryRepository
{
    Task<List<ProgresoSubtemaDto>> ObtenerProgresoPorSubtemaAsync(Guid alumnoId, Guid temaId, CancellationToken cancellationToken);
}
