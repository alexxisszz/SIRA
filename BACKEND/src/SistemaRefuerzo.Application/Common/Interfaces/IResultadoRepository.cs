using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Common.Interfaces;

public interface IResultadoRepository
{
    void Agregar(Resultado resultado);
}