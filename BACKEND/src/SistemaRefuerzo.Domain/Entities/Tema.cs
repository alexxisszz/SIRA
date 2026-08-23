namespace SistemaRefuerzo.Domain.Entities;

public class Tema
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public int Orden { get; private set; }

    private Tema() { }

    public Tema(string nombre, int orden)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Orden = orden;
    }

    public void ActualizarDatos(string nombre, int orden)
    {
        Nombre = nombre;
        Orden = orden;
    }
}