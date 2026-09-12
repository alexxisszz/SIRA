namespace SistemaRefuerzo.Domain.Entities;

/// <summary>
/// Registro de un ejercicio resuelto en modo práctica libre (fuera de una evaluación
/// formal de 10 preguntas): permite dar retroalimentación inmediata y conservar
/// trazabilidad de intentos sin afectar el avance de nivel, que depende únicamente
/// de aprobar las evaluaciones por nivel (<see cref="Evaluacion"/>).
/// </summary>
public class IntentoEjercicio
{
    public Guid Id { get; private set; }
    public Guid AlumnoId { get; private set; }
    public Guid TemaId { get; private set; }
    public string Subtema { get; private set; } = null!;
    public Guid PreguntaId { get; private set; }
    public Guid OpcionSeleccionadaId { get; private set; }
    public bool EsCorrecta { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    private IntentoEjercicio() { }

    public IntentoEjercicio(Guid alumnoId, Guid temaId, string subtema, Guid preguntaId, Guid opcionSeleccionadaId, bool esCorrecta)
    {
        Id = Guid.NewGuid();
        AlumnoId = alumnoId;
        TemaId = temaId;
        Subtema = subtema;
        PreguntaId = preguntaId;
        OpcionSeleccionadaId = opcionSeleccionadaId;
        EsCorrecta = esCorrecta;
        FechaRegistro = DateTime.UtcNow;
    }
}
