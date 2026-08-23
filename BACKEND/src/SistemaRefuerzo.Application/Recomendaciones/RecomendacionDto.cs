using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Recomendaciones;

public record EjercicioSugeridoDto(Guid Id, string Titulo);

public record RecomendacionDto(
    Guid Id,
    NivelDesempeno Nivel,
    List<string> TemasPorReforzar,
    List<EjercicioSugeridoDto> EjerciciosSugeridos,
    string Retroalimentacion);