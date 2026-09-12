using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class IntentoEjercicioConfiguration : IEntityTypeConfiguration<IntentoEjercicio>
{
    public void Configure(EntityTypeBuilder<IntentoEjercicio> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Property(i => i.Subtema).HasMaxLength(150).IsRequired();
        builder.HasIndex(i => i.AlumnoId);
        builder.HasIndex(i => i.PreguntaId);
        builder.HasIndex(i => new { i.AlumnoId, i.Subtema });
        builder.HasIndex(i => new { i.AlumnoId, i.TemaId });
    }
}
