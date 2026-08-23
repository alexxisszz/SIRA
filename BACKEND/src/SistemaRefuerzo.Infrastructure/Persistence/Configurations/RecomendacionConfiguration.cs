using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class RecomendacionConfiguration : IEntityTypeConfiguration<Recomendacion>
{
    public void Configure(EntityTypeBuilder<Recomendacion> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Nivel).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Retroalimentacion).HasMaxLength(2000).IsRequired();
        builder.HasIndex(r => r.ResultadoId).IsUnique();

        builder.Property(r => r.TemasPorReforzar)
            .HasField("_temasPorReforzar")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                temas => string.Join('|', temas),
                texto => texto.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<string>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, texto) => HashCode.Combine(hash, texto)),
                a => a.ToList()));

        builder.HasMany(r => r.EjerciciosRecomendados)
            .WithOne()
            .HasForeignKey(e => e.RecomendacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.EjerciciosRecomendados)
            .HasField("_ejerciciosRecomendados")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}