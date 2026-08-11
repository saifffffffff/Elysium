using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class AiChatConfiguration : IEntityTypeConfiguration<AiChat>
{
    public void Configure(EntityTypeBuilder<AiChat> builder)
    {
        builder.HasOne(c => c.StudentSession)
            .WithMany(ss => ss.AiChats)
            .HasForeignKey(c => c.StudentSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
