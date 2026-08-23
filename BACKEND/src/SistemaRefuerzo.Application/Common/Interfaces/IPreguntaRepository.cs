using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IPreguntaRepository
{
    Task<List<Pregunta>> ObtenerPorTemaAsync(Guid temaId, CancellationToken cancellationToken);
    Task<List<Pregunta>> ObtenerPorTemaYNivelAsync(Guid temaId, Domain.Enums.NivelDesempeno nivel, CancellationToken cancellationToken);
    Task<Pregunta?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    void Agregar(Pregunta pregunta);
    void Eliminar(Pregunta pregunta);
}
