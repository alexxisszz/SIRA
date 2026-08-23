using MediatR;

namespace SistemaRefuerzo.Application.Admin.Temas;

public record ActualizarTemaCommand(Guid TemaId, string Nombre, int Orden) : IRequest;
