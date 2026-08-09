using Elysium.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class ConfusionFlagConfiguration : IEntityTypeConfiguration<ConfusionFlag>
{
    public void Configure(EntityTypeBuilder<ConfusionFlag> builder)
    {
        builder.HasOne(cf => cf.StudentSession)
            .WithMany(ss => ss.ConfusionFlags)
            .HasForeignKey(cf => cf.StudentSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
