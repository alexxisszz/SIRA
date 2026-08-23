using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Repositories;

public class ResultadoRepository(AppDbContext dbContext) : IResultadoRepository
{
    public void Agregar(Resultado resultado) => dbContext.Resultados.Add(resultado);
}