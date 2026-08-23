using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IEvaluacionRepository
{
    Task<Evaluacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(Evaluacion evaluacion);
}