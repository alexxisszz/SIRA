namespace SistemaRefuerzo.Infrastructure.Security;

public class JwtSettings
{
    public const string SeccionConfiguracion = "Jwt";

    public string ClaveSecreta { get; set; } = null!;
    public string Emisor { get; set; } = null!;
    public string Audiencia { get; set; } = null!;
    public int MinutosExpiracion { get; set; } = 60;
}