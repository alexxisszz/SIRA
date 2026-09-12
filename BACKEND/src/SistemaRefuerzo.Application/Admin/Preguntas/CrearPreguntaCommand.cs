using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public record CrearPreguntaCommand(
    Guid TemaId,
    string Enunciado,
    string Subtema,
    NivelDesempeno NivelDificultad,
    TipoPregunta Tipo,
    string? Explicacion,
    int Puntaje,
    List<OpcionInput> Opciones) : IRequest<Guid>;
