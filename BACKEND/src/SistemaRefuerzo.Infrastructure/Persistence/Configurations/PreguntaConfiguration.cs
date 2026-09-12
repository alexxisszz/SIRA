using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class PreguntaConfiguration : IEntityTypeConfiguration<Pregunta>
{
    public void Configure(EntityTypeBuilder<Pregunta> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Enunciado).HasMaxLength(1000).IsRequired();
        builder.Property(p => p.Subtema).HasMaxLength(150).IsRequired();
        builder.Property(p => p.NivelDificultad).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Tipo).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.Explicacion).HasMaxLength(1000);
        builder.Property(p => p.Puntaje).IsRequired();
        builder.HasIndex(p => p.TemaId);
        builder.HasIndex(p => new { p.TemaId, p.Subtema });

        builder.HasMany(p => p.Opciones)
            .WithOne()
            .HasForeignKey(o => o.PreguntaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Opciones).HasField("_opciones").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}