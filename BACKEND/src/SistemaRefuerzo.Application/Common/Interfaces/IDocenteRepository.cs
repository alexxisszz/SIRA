using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IDocenteRepository
{
    Task<Docente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(Docente docente);
}
