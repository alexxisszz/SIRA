using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IRecomendacionRepository
{
    Task<Recomendacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Recomendacion?> ObtenerPorResultadoIdAsync(Guid resultadoId, CancellationToken cancellationToken);
    void Agregar(Recomendacion recomendacion);
}