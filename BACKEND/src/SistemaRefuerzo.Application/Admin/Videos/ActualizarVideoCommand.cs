using MediatR;

namespace SistemaRefuerzo.Application.Admin.Videos;

public record ActualizarVideoCommand(Guid VideoId, string Titulo, string Url) : IRequest;
