using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

public class Evaluacion
{
    private readonly List<RespuestaAlumno> _respuestas = [];
    private readonly List<Guid> _preguntasAsignadas = [];

    public Guid Id { get; private set; }
    public Guid TemaId { get; private set; }
    public Guid AlumnoId { get; private set; }
    public TipoEvaluacion Tipo { get; private set; }
    public NivelDesempeno? NivelEvaluado { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }
    public EstadoEvaluacion Estado { get; private set; }
    public IReadOnlyCollection<RespuestaAlumno> Respuestas => _respuestas.AsReadOnly();
    public IReadOnlyCollection<Guid> PreguntasAsignadas => _preguntasAsignadas.AsReadOnly();

    private Evaluacion() { }

    public Evaluacion(
        Guid temaId, Guid alumnoId, TipoEvaluacion tipo, IEnumerable<Guid> preguntasAsignadas, NivelDesempeno? nivelEvaluado = null)
    {
        Id = Guid.NewGuid();
        TemaId = temaId;
        AlumnoId = alumnoId;
        Tipo = tipo;
        NivelEvaluado = nivelEvaluado;
        FechaInicio = DateTime.UtcNow;
        Estado = EstadoEvaluacion.EnCurso;
        _preguntasAsignadas.AddRange(preguntasAsignadas);
    }

    public void RegistrarRespuesta(Guid preguntaId, Guid opcionSeleccionadaId, bool esCorrecta)
    {
        if (Estado == EstadoEvaluacion.Finalizada)
            throw new InvalidOperationException("No se pueden registrar respuestas en una evaluación finalizada.");

        if (!_preguntasAsignadas.Contains(preguntaId))
            throw new InvalidOperationException("La pregunta no pertenece a esta evaluación.");

        if (_respuestas.Any(r => r.PreguntaId == preguntaId))
            throw new InvalidOperationException("Ya registraste una respuesta para esta pregunta.");

        _respuestas.Add(new RespuestaAlumno(Id, preguntaId, opcionSeleccionadaId, esCorrecta));
    }

    public Resultado Finalizar()
    {
        if (Estado == EstadoEvaluacion.Finalizada)
            throw new InvalidOperationException("La evaluación ya fue finalizada.");

        if (_respuestas.Count != _preguntasAsignadas.Count)
            throw new InvalidOperationException("Debes responder todas las preguntas antes de finalizar.");

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
