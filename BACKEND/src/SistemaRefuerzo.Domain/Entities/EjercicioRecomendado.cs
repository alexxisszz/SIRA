namespace SistemaRefuerzo.Domain.Entities;

public class EjercicioRecomendado
{
    public Guid Id { get; private set; }
    public Guid RecomendacionId { get; private set; }
    public Guid PreguntaId { get; private set; }

    private EjercicioRecomendado() { }

    public EjercicioRecomendado(Guid recomendacionId, Guid preguntaId)
    {
        Id = Guid.NewGuid();
        RecomendacionId = recomendacionId;
        PreguntaId = preguntaId;
    }
}