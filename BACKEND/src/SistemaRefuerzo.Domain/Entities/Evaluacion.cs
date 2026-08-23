using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

public class Evaluacion
{
    private readonly List<RespuestaAlumno> _respuestas = [];

    public Guid Id { get; private set; }
    public Guid TemaId { get; private set; }
    public Guid AlumnoId { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }
    public EstadoEvaluacion Estado { get; private set; }
    public IReadOnlyCollection<RespuestaAlumno> Respuestas => _respuestas.AsReadOnly();

    private Evaluacion() { }

    public Evaluacion(Guid temaId, Guid alumnoId)
    {
        Id = Guid.NewGuid();
        TemaId = temaId;
        AlumnoId = alumnoId;
        FechaInicio = DateTime.UtcNow;
        Estado = EstadoEvaluacion.EnCurso;
    }

    public void RegistrarRespuesta(Guid preguntaId, Guid opcionSeleccionadaId, bool esCorrecta)
    {
        if (Estado == EstadoEvaluacion.Finalizada)
            throw new InvalidOperationException("No se pueden registrar respuestas en una evaluación finalizada.");

        _respuestas.Add(new RespuestaAlumno(Id, preguntaId, opcionSeleccionadaId, esCorrecta));
    }

    public Resultado Finalizar()
    {
        if (Estado == EstadoEvaluacion.Finalizada)
            throw new InvalidOperationException("La evaluación ya fue finalizada.");

        if (_respuestas.Count == 0)
            throw new InvalidOperationException("No se puede finalizar una evaluación sin respuestas.");

        Estado = EstadoEvaluacion.Finalizada;
        FechaFin = DateTime.UtcNow;

        var puntaje = CalcularPuntaje();
        var fallosConsecutivos = CalcularFallosConsecutivosMaximos();

        return new Resultado(Id, puntaje, fallosConsecutivos);
    }

    private int CalcularPuntaje()
    {
        var correctas = _respuestas.Count(r => r.EsCorrecta);
        return (int)Math.Round(correctas * 100.0 / _respuestas.Count);
    }

    private int CalcularFallosConsecutivosMaximos()
    {
        var maximo = 0;
        var actual = 0;
        foreach (var respuesta in _respuestas.OrderBy(r => r.FechaRegistro))
        {
            actual = respuesta.EsCorrecta ? 0 : actual + 1;
            maximo = Math.Max(maximo, actual);
        }
        return maximo;
    }
}