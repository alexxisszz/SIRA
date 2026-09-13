using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

public class Recomendacion
{
    private readonly List<string> _temasPorReforzar = [];
    private readonly List<string> _subtemasDominados = [];
    private readonly List<string> _reglasAplicadas = [];
    private readonly List<EjercicioRecomendado> _ejerciciosRecomendados = [];

    public Guid Id { get; private set; }
    public Guid ResultadoId { get; private set; }
    public NivelDesempeno Nivel { get; private set; }
    public string Retroalimentacion { get; private set; } = null!;
    public DateTime FechaGeneracion { get; private set; }
    public IReadOnlyCollection<string> TemasPorReforzar => _temasPorReforzar.AsReadOnly();
    public IReadOnlyCollection<string> SubtemasDominados => _subtemasDominados.AsReadOnly();

    /// <summary>Nombres de las reglas del motor de inferencia que se dispararon al generar esta recomendación (trazabilidad).</summary>
    public IReadOnlyCollection<string> ReglasAplicadas => _reglasAplicadas.AsReadOnly();
    public IReadOnlyCollection<EjercicioRecomendado> EjerciciosRecomendados => _ejerciciosRecomendados.AsReadOnly();

    private Recomendacion() { }

    public Recomendacion(
        Guid resultadoId,
        NivelDesempeno nivel,
        string retroalimentacion,
        IEnumerable<string> temasPorReforzar,
        IEnumerable<string> subtemasDominados,
        IEnumerable<string>? reglasAplicadas = null)
    {
        Id = Guid.NewGuid();
        ResultadoId = resultadoId;
        Nivel = nivel;
        Retroalimentacion = retroalimentacion;
        FechaGeneracion = DateTime.UtcNow;
        _temasPorReforzar.AddRange(temasPorReforzar);
        _subtemasDominados.AddRange(subtemasDominados);
        _reglasAplicadas.AddRange(reglasAplicadas ?? []);
    }

    public void AgregarEjercicioRecomendado(Guid preguntaId)
    {
        _ejerciciosRecomendados.Add(new EjercicioRecomendado(Id, preguntaId));
    }
}