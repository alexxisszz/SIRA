using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IReglaRepository
{
    Task<List<Regla>> ObtenerActivasAsync(CancellationToken cancellationToken);
    Task<List<Regla>> ObtenerTodasAsync(CancellationToken cancellationToken);
    Task<Regla?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
}
