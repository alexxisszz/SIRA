using SistemaRefuerzo.Application.Common.Exceptions;

namespace SistemaRefuerzo.Application.Admin.Videos;

public static class ValidadorDeUrlVideo
{
    public static void Validar(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ReglaDeNegocioException("La URL del video es obligatoria.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ReglaDeNegocioException("La URL del video no es válida.");

        var host = uri.Host.Replace("www.", string.Empty, StringComparison.OrdinalIgnoreCase);
        if (host != "youtube.com" && host != "youtu.be" && host != "m.youtube.com")
            throw new ReglaDeNegocioException("Por ahora solo se admiten videos de YouTube.");
    }
}
