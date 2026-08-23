using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface ITemaRepository
{
    Task<List<Tema>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<Tema?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(Tema tema);
}
