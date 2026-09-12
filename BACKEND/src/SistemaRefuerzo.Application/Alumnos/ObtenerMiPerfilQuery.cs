using MediatR;

namespace SistemaRefuerzo.Application.Alumnos;

public record ObtenerMiPerfilQuery(Guid UsuarioId) : IRequest<PerfilAlumnoDto>;
