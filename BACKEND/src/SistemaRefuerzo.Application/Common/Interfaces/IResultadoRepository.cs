using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IResultadoRepository
{
    Task<Resultado?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Resultado?> ObtenerPorEvaluacionIdAsync(Guid evaluacionId, CancellationToken cancellationToken);
    void Agregar(Resultado resultado);
}
