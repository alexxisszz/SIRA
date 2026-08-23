using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Security;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtSettings) : IJwtTokenGenerator
{
    public string GenerarToken(Usuario usuario)
    {
        var settings = jwtSettings.Value;
        var claves = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.ClaveSecreta));
        var credenciales = new SigningCredentials(claves, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.CorreoElectronico),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: settings.Emisor,
            audience: settings.Audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.MinutosExpiracion),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}