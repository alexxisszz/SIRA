using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class FichaRegistroNotaConfiguration : IEntityTypeConfiguration<FichaRegistroNota>
{
    private const int Precision = 5;
    private const int Escala = 2;

    public void Configure(EntityTypeBuilder<FichaRegistroNota> builder)
    {
        builder.ToTable("FichasRegistroNotas");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();
        builder.Property(f => f.TipoEvaluacion).HasConversion<string>().HasMaxLength(20);

        builder.Property(f => f.D1I1).HasPrecision(Precision, Escala);
        builder.Property(f => f.D1I2).HasPrecision(Precision, Escala);
        builder.Property(f => f.D1I3).HasPrecision(Precision, Escala);
        builder.Property(f => f.D2I1).HasPrecision(Precision, Escala);
        builder.Property(f => f.D2I2).HasPrecision(Precision, Escala);
        builder.Property(f => f.D2I3).HasPrecision(Precision, Escala);
        builder.Property(f => f.D3I1).HasPrecision(Precision, Escala);
        builder.Property(f => f.D3I2).HasPrecision(Precision, Escala);
        builder.Property(f => f.D3I3).HasPrecision(Precision, Escala);

        builder.HasIndex(f => new { f.AlumnoId, f.TemaId, f.TipoEvaluacion }).IsUnique();
        builder.HasIndex(f => new { f.TemaId, f.TipoEvaluacion });
    }
}
