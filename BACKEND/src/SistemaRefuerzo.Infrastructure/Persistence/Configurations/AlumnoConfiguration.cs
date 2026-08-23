using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class AlumnoConfiguration : IEntityTypeConfiguration<Alumno>
{
    public void Configure(EntityTypeBuilder<Alumno> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.HasIndex(a => a.UsuarioId).IsUnique();
        builder.Property(a => a.Nombres).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Grado).HasMaxLength(50).IsRequired();
    }
}