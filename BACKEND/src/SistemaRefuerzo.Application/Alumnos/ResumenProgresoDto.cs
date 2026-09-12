namespace SistemaRefuerzo.Application.Alumnos;

public record ResumenProgresoDto(
    int EjerciciosIntentados,
    int EjerciciosCorrectos,
    int SubtemasTrabajados,
    int DiasConActividad,
    DateTime? UltimaActividad);
