using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IEvaluacionRepository
{
    Task<Evaluacion?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Evaluacion?> ObtenerUltimaFinalizadaAsync(
        Guid alumnoId, Guid temaId, TipoEvaluacion tipo, NivelDesempeno? nivel, CancellationToken cancellationToken);

    void Agregar(Evaluacion evaluacion);
}
