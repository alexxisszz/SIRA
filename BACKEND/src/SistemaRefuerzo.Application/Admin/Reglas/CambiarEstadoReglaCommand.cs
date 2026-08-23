using MediatR;

namespace SistemaRefuerzo.Application.Admin.Reglas;

public record CambiarEstadoReglaCommand(Guid ReglaId, bool Activa) : IRequest;
