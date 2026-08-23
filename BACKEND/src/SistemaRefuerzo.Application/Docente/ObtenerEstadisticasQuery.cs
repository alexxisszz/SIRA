using MediatR;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerEstadisticasQuery : IRequest<EstadisticasDto>;
