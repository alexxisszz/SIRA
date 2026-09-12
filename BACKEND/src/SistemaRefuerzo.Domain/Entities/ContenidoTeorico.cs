using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

/// <summary>
/// Contenido teórico administrable de un tema: puede ser una introducción general de un
/// nivel de dificultad (<see cref="TipoContenidoTeorico.NivelGeneral"/>, <see cref="Clave"/> =
/// nombre del <c>NivelDesempeno</c>) o el refuerzo específico de un subtema
/// (<see cref="TipoContenidoTeorico.Subtema"/>, <see cref="Clave"/> = nombre del subtema).
/// </summary>
public class ContenidoTeorico
{
    private readonly List<string> _parrafos = [];

    public Guid Id { get; private set; }
    public Guid TemaId { get; private set; }
    public TipoContenidoTeorico Tipo { get; private set; }
    public string Clave { get; private set; } = null!;
    public string Titulo { get; private set; } = null!;
    public IReadOnlyCollection<string> Parrafos => _parrafos.AsReadOnly();

    private ContenidoTeorico() { }

    public ContenidoTeorico(Guid temaId, TipoContenidoTeorico tipo, string clave, string titulo, IEnumerable<string> parrafos)
    {
        Id = Guid.NewGuid();
        TemaId = temaId;
        Tipo = tipo;
        Clave = clave;
        Titulo = titulo;
        _parrafos.AddRange(parrafos);
    }

    public void ActualizarContenido(string clave, string titulo, IEnumerable<string> parrafos)
    {
        Clave = clave;
        Titulo = titulo;
        _parrafos.Clear();
        _parrafos.AddRange(parrafos);
    }
}
