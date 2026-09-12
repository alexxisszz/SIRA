using MediatR;

namespace SistemaRefuerzo.Application.Admin.Videos;

public record ObtenerVideosAdminQuery(Guid TemaId) : IRequest<List<AdminVideoApoyoDto>>;
