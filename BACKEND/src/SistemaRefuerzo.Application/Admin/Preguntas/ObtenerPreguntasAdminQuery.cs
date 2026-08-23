using MediatR;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public record ObtenerPreguntasAdminQuery(Guid TemaId) : IRequest<List<AdminPreguntaDto>>;
