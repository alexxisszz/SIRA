using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Recomendaciones;

public record EjercicioSugeridoDto(Guid Id, string Titulo);

public record RespuestaDetalleDto(
    Guid PreguntaId,
    string Enunciado,
    string OpcionSeleccionadaTexto,
    string OpcionCorrectaTexto,
    bool EsCorrecta);

public record RecomendacionDto(
    Guid Id,
    Guid TemaId,
    int Puntaje,
    NivelDesempeno Nivel,
    List<string> TemasPorReforzar,
    List<string> SubtemasDominados,
    List<EjercicioSugeridoDto> EjerciciosSugeridos,
    string Retroalimentacion,
    List<RespuestaDetalleDto> RespuestasDetalle,
    ReporteProgresoTemaDto? ReporteProgreso);
