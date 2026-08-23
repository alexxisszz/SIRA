using MediatR;

namespace SistemaRefuerzo.Application.Recomendaciones;

public record ObtenerRecomendacionQuery(Guid RecomendacionId) : IRequest<RecomendacionDto>;