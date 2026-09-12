using MediatR;

namespace SistemaRefuerzo.Application.Alumnos;

public record ObtenerResumenProgresoQuery(Guid UsuarioId) : IRequest<ResumenProgresoDto>;
