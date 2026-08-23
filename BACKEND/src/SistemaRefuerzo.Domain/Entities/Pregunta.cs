using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

public class Pregunta
{
    private readonly List<OpcionPregunta> _opciones = [];

    public Guid Id { get; private set; }
    public Guid TemaId { get; private set; }
    public string Enunciado { get; private set; } = null!;
    public NivelDesempeno NivelDificultad { get; private set; }
    public IReadOnlyCollection<OpcionPregunta> Opciones => _opciones.AsReadOnly();

    private Pregunta() { }

    public Pregunta(Guid temaId, string enunciado, NivelDesempeno nivelDificultad)
    {
        Id = Guid.NewGuid();
        TemaId = temaId;
        Enunciado = enunciado;
        NivelDificultad = nivelDificultad;
    }

    public void AgregarOpcion(string texto, bool esCorrecta)
    {
        _opciones.Add(new OpcionPregunta(Id, texto, esCorrecta));
    }

    public void ActualizarContenido(string enunciado, NivelDesempeno nivelDificultad, IEnumerable<(string Texto, bool EsCorrecta)> opciones)
    {
        Enunciado = enunciado;
        NivelDificultad = nivelDificultad;

        _opciones.Clear();
        foreach (var opcion in opciones)
            AgregarOpcion(opcion.Texto, opcion.EsCorrecta);
    }
}
