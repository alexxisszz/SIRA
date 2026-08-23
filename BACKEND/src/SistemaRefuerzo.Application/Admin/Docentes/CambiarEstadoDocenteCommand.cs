using MediatR;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public record CambiarEstadoDocenteCommand(Guid DocenteId, bool Activo) : IRequest;
