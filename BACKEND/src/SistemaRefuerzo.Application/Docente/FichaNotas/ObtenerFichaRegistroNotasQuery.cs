using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record ObtenerFichaRegistroNotasQuery(Guid TemaId, TipoEvaluacion TipoEvaluacion) : IRequest<FichaRegistroNotasDto>;
