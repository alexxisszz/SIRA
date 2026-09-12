using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IIntentoEjercicioRepository
{
    void Agregar(IntentoEjercicio intento);

    /// <summary>Últimos intentos del alumno en un subtema, ordenados del más reciente al más antiguo.</summary>
    Task<List<IntentoEjercicio>> ObtenerUltimosPorAlumnoYSubtemaAsync(
        Guid alumnoId, string subtema, int cantidad, CancellationToken cancellationToken);

    /// <summary>Todos los intentos de práctica libre del alumno, en cualquier tema.</summary>
    Task<List<IntentoEjercicio>> ObtenerPorAlumnoAsync(Guid alumnoId, CancellationToken cancellationToken);
}
