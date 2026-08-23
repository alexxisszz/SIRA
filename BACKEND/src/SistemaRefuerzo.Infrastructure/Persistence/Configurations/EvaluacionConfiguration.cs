using Microsoft.EntityFrameworkCore;
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
        builder.HasIndex(e => e.AlumnoId);
        builder.HasIndex(e => e.TemaId);

        builder.HasMany(e => e.Respuestas)
            .WithOne()
            .HasForeignKey(r => r.EvaluacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Respuestas).HasField("_respuestas").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}