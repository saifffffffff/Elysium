using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class TranscriptSegmentConfiguration : IEntityTypeConfiguration<TranscriptSegment>
{
    public void Configure(EntityTypeBuilder<TranscriptSegment> builder)
    {
        builder.HasOne(ts => ts.Session)
            .WithMany(s => s.TranscriptSegments)
            .HasForeignKey(ts => ts.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("TranscriptSegments", t =>
        {
            t.HasCheckConstraint("CK_TranscriptSegments_times", "EndTime > StartTime");
            t.HasCheckConstraint("CK_TranscriptSegments_startTime", "StartTime >= 0");
            t.HasCheckConstraint("CK_TranscriptSegments_text", "LEN(TRIM(Text)) > 0");
        });
    }
}
