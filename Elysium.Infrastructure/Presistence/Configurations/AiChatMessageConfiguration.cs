using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Presistence.Configurations;

public class AiChatMessageConfiguration : IEntityTypeConfiguration<AiChatMessage>
{
    public void Configure(EntityTypeBuilder<AiChatMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Answer)
            .IsRequired(false);

        builder.Property(x => x.AskedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.AnsweredAt)
            .IsRequired(false);

        builder.HasOne(x => x.StudentSession)
            .WithMany(ss => ss.AiChatMessages)
            .HasForeignKey(x => x.StudentSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("AiChatMessages", t =>
        {
            t.HasCheckConstraint("CK_AiChatMessages_Question", "LEN(TRIM(Question)) > 0");
            t.HasCheckConstraint("CK_AiChatMessages_Dates", "(AnsweredAt IS NULL OR AnsweredAt > AskedAt)");
        });
    }
}
