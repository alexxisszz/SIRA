using MediatR;

namespace SistemaRefuerzo.Application.Admin.Docentes;

public record ActualizarDocenteCommand(Guid DocenteId, string Nombres, string Apellidos) : IRequest;
