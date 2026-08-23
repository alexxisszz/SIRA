using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.CorreoElectronico).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.CorreoElectronico).IsUnique();

        builder.Property(u => u.ContrasenaHash).IsRequired();
        builder.Property(u => u.Rol).HasConversion<string>().HasMaxLength(20);
    }
}