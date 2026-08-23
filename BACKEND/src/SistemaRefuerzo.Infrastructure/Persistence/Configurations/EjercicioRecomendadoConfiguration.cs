using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class EjercicioRecomendadoConfiguration : IEntityTypeConfiguration<EjercicioRecomendado>
{
    public void Configure(EntityTypeBuilder<EjercicioRecomendado> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
    }
}