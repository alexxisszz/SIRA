using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

public record ObtenerPreguntasPorTemaQuery(
    Guid TemaId,
    NivelDesempeno? Nivel,
    string? Subtema = null,
    List<string>? ExcluirSubtemas = null) : IRequest<List<PreguntaDto>>;
