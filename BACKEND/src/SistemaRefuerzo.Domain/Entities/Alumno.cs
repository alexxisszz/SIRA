namespace SistemaRefuerzo.Domain.Entities;

public class Alumno
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Nombres { get; private set; } = null!;
    public string Apellidos { get; private set; } = null!;
    public string Grado { get; private set; } = null!;

    private Alumno() { }

    public Alumno(Guid usuarioId, string nombres, string apellidos, string grado)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        Nombres = nombres;
        Apellidos = apellidos;
        Grado = grado;
    }

    public void ActualizarDatos(string nombres, string apellidos, string grado)
    {
        Nombres = nombres;
        Apellidos = apellidos;
        Grado = grado;
    }
}