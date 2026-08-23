using MediatR;

namespace SistemaRefuerzo.Application.Admin.Reglas;

public record ObtenerReglasAdminQuery : IRequest<List<AdminReglaDto>>;
