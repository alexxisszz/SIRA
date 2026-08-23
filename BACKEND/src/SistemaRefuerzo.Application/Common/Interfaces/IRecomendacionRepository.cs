using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IRecomendacionRepository
{
    Task<Recomendacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(Recomendacion recomendacion);
}