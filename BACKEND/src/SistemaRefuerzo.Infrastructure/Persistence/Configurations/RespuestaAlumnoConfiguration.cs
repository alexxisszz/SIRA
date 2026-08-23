using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class RespuestaAlumnoConfiguration : IEntityTypeConfiguration<RespuestaAlumno>
{
    public void Configure(EntityTypeBuilder<RespuestaAlumno> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
    }
}