using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class AiChatMessageConfiguration : IEntityTypeConfiguration<AiChatMessage>
{
    public void Configure(EntityTypeBuilder<AiChatMessage> builder)
    {
        builder.HasOne(m => m.AiChat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.AiChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
