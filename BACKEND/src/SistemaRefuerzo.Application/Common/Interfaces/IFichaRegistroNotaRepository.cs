using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IFichaRegistroNotaRepository
{
    Task<FichaRegistroNota?> ObtenerAsync(Guid alumnoId, Guid temaId, TipoEvaluacion tipo, CancellationToken cancellationToken);
    Task GuardarAsync(FichaRegistroNota ficha, CancellationToken cancellationToken);
    Task<List<FichaRegistroNota>> ListarPorTemaAsync(Guid temaId, TipoEvaluacion tipo, CancellationToken cancellationToken);
}
