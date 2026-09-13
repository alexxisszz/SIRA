using MediatR;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerResumenGrupoQuery(Guid? TemaId = null) : IRequest<ResumenGrupoDto>;
