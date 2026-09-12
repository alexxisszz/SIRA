using MediatR;

namespace SistemaRefuerzo.Application.Admin.Videos;

public record CrearVideoCommand(Guid TemaId, string Titulo, string Url) : IRequest<Guid>;
