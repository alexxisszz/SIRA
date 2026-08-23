namespace SistemaRefuerzo.Domain.Entities;

public class Docente
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Nombres { get; private set; } = null!;
    public string Apellidos { get; private set; } = null!;

    private Docente() { }

    public Docente(Guid usuarioId, string nombres, string apellidos)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        Nombres = nombres;
        Apellidos = apellidos;
    }

    public void ActualizarDatos(string nombres, string apellidos)
    {
        Nombres = nombres;
        Apellidos = apellidos;
    }
}