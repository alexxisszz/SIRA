using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class EvaluacionConfiguration : IEntityTypeConfiguration<Evaluacion>
{
    public void Configure(EntityTypeBuilder<Evaluacion> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.NivelEvaluado).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => e.AlumnoId);
        builder.HasIndex(e => e.TemaId);

        builder.Property(e => e.PreguntasAsignadas)
            .HasField("_preguntasAsignadas")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                ids => string.Join('|', ids),
                texto => texto.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<Guid>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, id) => HashCode.Combine(hash, id)),
                a => a.ToList()));

        builder.HasMany(e => e.Respuestas)
            .WithOne()
            .HasForeignKey(r => r.EvaluacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Respuestas).HasField("_respuestas").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}