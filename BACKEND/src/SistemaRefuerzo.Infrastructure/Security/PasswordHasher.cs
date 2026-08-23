using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hashear(string contrasena) => BCrypt.Net.BCrypt.HashPassword(contrasena);

    public bool Verificar(string contrasena, string hash) => BCrypt.Net.BCrypt.Verify(contrasena, hash);
}