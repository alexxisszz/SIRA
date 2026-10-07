using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin;

public record AdminOpcionDto(Guid Id, string Texto, bool EsCorrecta);

public record AdminPreguntaDto(
    Guid Id,
    Guid TemaId,
    string Enunciado,
    string Subtema,
    NivelDesempeno NivelDificultad,
    IndicadorCognitivo Indicador,
    TipoPregunta Tipo,
    string? Explicacion,
    int Puntaje,
    List<AdminOpcionDto> Opciones);
