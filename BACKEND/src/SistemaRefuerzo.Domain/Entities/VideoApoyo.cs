namespace SistemaRefuerzo.Domain.Entities;

public class VideoApoyo
{
    public Guid Id { get; private set; }
    public Guid TemaId { get; private set; }
    public string Titulo { get; private set; } = null!;
    public string Url { get; private set; } = null!;

    private VideoApoyo() { }

    public VideoApoyo(Guid temaId, string titulo, string url)
    {
        Id = Guid.NewGuid();
        TemaId = temaId;
        Titulo = titulo;
        Url = url;
    }

    public void ActualizarContenido(string titulo, string url)
    {
        Titulo = titulo;
        Url = url;
    }
}
