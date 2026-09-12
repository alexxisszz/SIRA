using MediatR;

namespace SistemaRefuerzo.Application.Ejercicios;

public record ObtenerProgresoPracticaQuery(Guid TemaId, Guid UsuarioId) : IRequest<List<ProgresoSubtemaDto>>;
