namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hashear(string contrasena);
    bool Verificar(string contrasena, string hash);
}