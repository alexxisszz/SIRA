using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Recomendaciones;

public record ProgresoSubtemaComparadoDto(string Subtema, double PorcentajeInicial, double PorcentajeFinal);

public record ReporteProgresoTemaDto(
    int PuntajeInicial,
    int PuntajeFinal,
    NivelDesempeno? NivelInicial,
    NivelDesempeno NivelFinal,
    List<ProgresoSubtemaComparadoDto> Subtemas);
