using MediatR;

namespace SistemaRefuerzo.Application.Recomendaciones;

public record ObtenerRecomendacionQuery(Guid RecomendacionId, Guid UsuarioId) : IRequest<RecomendacionDto>;
