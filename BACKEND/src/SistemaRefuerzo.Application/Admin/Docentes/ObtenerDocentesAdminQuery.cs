using MediatR;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public record ObtenerDocentesAdminQuery : IRequest<List<AdminDocenteDto>>;
