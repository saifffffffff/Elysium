using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {


        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(512);


        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnType("tinyint")
            .HasDefaultValue(SessionStatus.Live);

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.FinishedAt)
            .IsRequired(false);

        builder.ToTable("Sessions",
            t =>
            {
                t.HasCheckConstraint("CK_Sessions_Dates", "(FinishedAt > StartedAt)");
                t.HasCheckConstraint("CK_Sessions_Name" , "(LEN(TRIM(name)) > 0)");

            });


        builder.HasOne(x => x.Course).WithMany(x => x.Sessions).HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Materials).WithOne(x => x.Session).HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TranscriptSegments).WithOne(x => x.Session).HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StudentSessions).WithOne(x => x.Session).HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
