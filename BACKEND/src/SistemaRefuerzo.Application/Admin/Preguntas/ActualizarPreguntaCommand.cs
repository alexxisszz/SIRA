using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public record ActualizarPreguntaCommand(
    Guid PreguntaId,
    string Enunciado,
    string Subtema,
    NivelDesempeno NivelDificultad,
    IndicadorCognitivo Indicador,
    TipoPregunta Tipo,
    string? Explicacion,
    int Puntaje,
    List<OpcionInput> Opciones) : IRequest;
