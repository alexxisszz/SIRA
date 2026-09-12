using MediatR;

namespace SistemaRefuerzo.Application.Admin.Videos;

public record EliminarVideoCommand(Guid VideoId) : IRequest;
