using MediatR;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record ObtenerEstadoTemaQuery(Guid TemaId, Guid UsuarioId) : IRequest<EstadoTemaDto>;
