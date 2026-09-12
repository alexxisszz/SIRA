using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence.Configurations;

public class VideoApoyoConfiguration : IEntityTypeConfiguration<VideoApoyo>
{
    public void Configure(EntityTypeBuilder<VideoApoyo> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();
        builder.Property(v => v.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(v => v.Url).HasMaxLength(500).IsRequired();
        builder.HasIndex(v => v.TemaId);
    }
}
