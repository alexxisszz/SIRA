using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class ResultadoConfiguration : IEntityTypeConfiguration<Resultado>
{
    public void Configure(EntityTypeBuilder<Resultado> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.HasIndex(r => r.EvaluacionId).IsUnique();
    }
}