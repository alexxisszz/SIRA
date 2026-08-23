namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}