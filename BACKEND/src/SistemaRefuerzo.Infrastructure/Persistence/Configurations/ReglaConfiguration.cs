using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class ReglaConfiguration : IEntityTypeConfiguration<Regla>
{
    public void Configure(EntityTypeBuilder<Regla> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(r => r.NombreClaseRegla).HasMaxLength(150).IsRequired();
        builder.Property(r => r.DescripcionCondicion).HasMaxLength(500).IsRequired();
        builder.Property(r => r.DescripcionConclusion).HasMaxLength(500).IsRequired();
    }
}