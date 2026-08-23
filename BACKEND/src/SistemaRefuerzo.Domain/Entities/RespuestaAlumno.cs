namespace SistemaRefuerzo.Domain.Entities;

public class RespuestaAlumno
{
    public Guid Id { get; private set; }
    public Guid EvaluacionId { get; private set; }
    public Guid PreguntaId { get; private set; }
    public Guid OpcionSeleccionadaId { get; private set; }
    public bool EsCorrecta { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    private RespuestaAlumno() { }

    public RespuestaAlumno(Guid evaluacionId, Guid preguntaId, Guid opcionSeleccionadaId, bool esCorrecta)
    {
        Id = Guid.NewGuid();
        EvaluacionId = evaluacionId;
        PreguntaId = preguntaId;
        OpcionSeleccionadaId = opcionSeleccionadaId;
        EsCorrecta = esCorrecta;
        FechaRegistro = DateTime.UtcNow;
    }
}