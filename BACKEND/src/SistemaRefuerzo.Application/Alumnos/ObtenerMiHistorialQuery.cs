using MediatR;
using SistemaRefuerzo.Application.Reportes.Docente;

namespace SistemaRefuerzo.Application.Alumnos;

public record ObtenerMiHistorialQuery(Guid UsuarioId) : IRequest<List<ResultadoHistoricoDto>>;
