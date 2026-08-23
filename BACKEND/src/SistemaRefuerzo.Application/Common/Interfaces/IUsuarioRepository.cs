using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico, CancellationToken cancellationToken);
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Alumno?> ObtenerAlumnoPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken);
    void Agregar(Usuario usuario);
}
