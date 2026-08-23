using MediatR;

namespace SistemaRefuerzo.Application.Admin.Temas;

public record CrearTemaCommand(string Nombre, int Orden) : IRequest<Guid>;
