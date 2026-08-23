using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin;

public record AdminOpcionDto(Guid Id, string Texto, bool EsCorrecta);

public record AdminPreguntaDto(
    Guid Id,
    Guid TemaId,
    string Enunciado,
    NivelDesempeno NivelDificultad,
    List<AdminOpcionDto> Opciones);
