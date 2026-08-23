namespace SistemaRefuerzo.Domain.Entities;

public class OpcionPregunta
{
    public Guid Id { get; private set; }
    public Guid PreguntaId { get; private set; }
    public string Texto { get; private set; } = null!;
    public bool EsCorrecta { get; private set; }

    private OpcionPregunta() { }

    public OpcionPregunta(Guid preguntaId, string texto, bool esCorrecta)
    {
        Id = Guid.NewGuid();
        PreguntaId = preguntaId;
        Texto = texto;
        EsCorrecta = esCorrecta;
    }
}