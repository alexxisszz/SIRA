using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string CorreoElectronico { get; private set; } = null!;
    public string ContrasenaHash { get; private set; } = null!;
    public Rol Rol { get; private set; }
    public bool Activo { get; private set; }

    private Usuario() { }

    public Usuario(string correoElectronico, string contrasenaHash, Rol rol)
    {
        if (string.IsNullOrWhiteSpace(correoElectronico))
            throw new ArgumentException("El correo electrónico es obligatorio.", nameof(correoElectronico));

        Id = Guid.NewGuid();
        CorreoElectronico = correoElectronico;
        ContrasenaHash = contrasenaHash;
        Rol = rol;
        Activo = true;
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}