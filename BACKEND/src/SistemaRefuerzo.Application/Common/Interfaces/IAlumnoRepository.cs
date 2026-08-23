using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IAlumnoRepository
{
    Task<Alumno?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(Alumno alumno);
}
