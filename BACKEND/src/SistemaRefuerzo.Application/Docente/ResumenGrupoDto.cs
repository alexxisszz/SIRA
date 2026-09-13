using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public record NivelDistribucionDto(NivelDesempeno Nivel, int Cantidad);

public record DificultadGrupoDto(string Subtema, double PorcentajeError, int TotalIntentos);

public record AlertaAlumnoDto(Guid AlumnoId, string Nombres, string Apellidos, NivelDesempeno? NivelActual, string Motivo);

public record RendimientoAlumnoDto(
    Guid AlumnoId,
    string Nombres,
    string Apellidos,
    int? PuntajeEntrada,
    NivelDesempeno? NivelInicial,
    NivelDesempeno? NivelActual,
    double PorcentajeAvance,
    int? UltimoPuntaje,
    DateTime? UltimaActividad,
    string Estado);

public record ResumenGrupoDto(
    int TotalEstudiantes,
    double PromedioGeneral,
    double AvancePromedio,
    int EstudiantesRequierenApoyo,
    List<NivelDistribucionDto> DistribucionNiveles,
    List<DificultadGrupoDto> DificultadesDelGrupo,
    List<AlertaAlumnoDto> Alertas,
    List<RendimientoAlumnoDto> Rendimiento);
