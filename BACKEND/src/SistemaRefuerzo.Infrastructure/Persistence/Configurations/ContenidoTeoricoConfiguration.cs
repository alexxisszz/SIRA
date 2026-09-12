using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class ContenidoTeoricoConfiguration : IEntityTypeConfiguration<ContenidoTeorico>
{
    private const char SeparadorParrafos = '␟';

    public void Configure(EntityTypeBuilder<ContenidoTeorico> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Clave).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Titulo).HasMaxLength(200).IsRequired();
        builder.HasIndex(c => new { c.TemaId, c.Tipo, c.Clave });

        builder.Property(c => c.Parrafos)
            .HasField("_parrafos")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                parrafos => string.Join(SeparadorParrafos, parrafos),
                texto => texto.Split(SeparadorParrafos, StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<string>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, texto) => HashCode.Combine(hash, texto)),
                a => a.ToList()));
    }
}
