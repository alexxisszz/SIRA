using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IAlumnoRepository
{
    Task<Alumno?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Alumno>> ListarAsync(CancellationToken cancellationToken);
    void Agregar(Alumno alumno);
}
