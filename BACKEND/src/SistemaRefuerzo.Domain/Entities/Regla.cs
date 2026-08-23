namespace SistemaRefuerzo.Domain.Entities;

/// <summary>
/// Representación administrable de una regla de la Base de Conocimiento: permite
/// al administrador ver, documentar y activar/desactivar las reglas que ejecuta
/// el Motor de Inferencia (identificadas por <see cref="NombreClaseRegla"/>).
/// </summary>
public class Regla
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string NombreClaseRegla { get; private set; } = null!;
    public string DescripcionCondicion { get; private set; } = null!;
    public string DescripcionConclusion { get; private set; } = null!;
    public int Prioridad { get; private set; }
    public bool Activa { get; private set; }

    private Regla() { }

    public Regla(string nombre, string nombreClaseRegla, string descripcionCondicion, string descripcionConclusion, int prioridad)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        NombreClaseRegla = nombreClaseRegla;
        DescripcionCondicion = descripcionCondicion;
        DescripcionConclusion = descripcionConclusion;
        Prioridad = prioridad;
        Activa = true;
    }

    public void Activar() => Activa = true;
    public void Desactivar() => Activa = false;

    public void ActualizarMetadata(string nombre, string descripcionCondicion, string descripcionConclusion, int prioridad)
    {
        Nombre = nombre;
        DescripcionCondicion = descripcionCondicion;
        DescripcionConclusion = descripcionConclusion;
        Prioridad = prioridad;
    }
}