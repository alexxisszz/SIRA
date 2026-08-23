namespace SistemaRefuerzo.Domain.Entities;

public class Resultado
{
    public Guid Id { get; private set; }
    public Guid EvaluacionId { get; private set; }
    public int Puntaje { get; private set; }
    public int FallosConsecutivos { get; private set; }
    public DateTime FechaCalculo { get; private set; }

    private Resultado() { }

    public Resultado(Guid evaluacionId, int puntaje, int fallosConsecutivos)
    {
        Id = Guid.NewGuid();
        EvaluacionId = evaluacionId;
        Puntaje = puntaje;
        FallosConsecutivos = fallosConsecutivos;
        FechaCalculo = DateTime.UtcNow;
    }
}