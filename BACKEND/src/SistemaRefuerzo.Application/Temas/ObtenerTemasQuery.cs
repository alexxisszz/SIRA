using MediatR;

namespace SistemaRefuerzo.Application.Temas;

public record ObtenerTemasQuery : IRequest<List<TemaDto>>;