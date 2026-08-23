using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class DocenteConfiguration : IEntityTypeConfiguration<Docente>
{
    public void Configure(EntityTypeBuilder<Docente> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();
        builder.HasIndex(d => d.UsuarioId).IsUnique();
        builder.Property(d => d.Nombres).HasMaxLength(150).IsRequired();
        builder.Property(d => d.Apellidos).HasMaxLength(150).IsRequired();
    }
}