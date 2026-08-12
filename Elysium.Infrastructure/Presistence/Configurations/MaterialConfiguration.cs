using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Presistence.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.Property(m => m.Name).HasMaxLength(256);
        builder.Property(m => m.StoredPath).HasMaxLength(500);
        builder.Property(m => m.Extension).HasMaxLength(16);
        builder.Property(m => m.ContentType).HasMaxLength(64);

        builder.HasOne(m => m.Session)
            .WithMany(s => s.Materials)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Materials", t =>
        {
            t.HasCheckConstraint("CK_Materials_sizeBytes", "SizeBytes > 0");
            t.HasCheckConstraint("CK_Materials_extension", "LEN(TRIM(Extension)) > 0");
        });
    }
}
