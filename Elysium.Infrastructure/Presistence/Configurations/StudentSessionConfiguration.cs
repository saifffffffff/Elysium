using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Presistence.Configurations;

public class StudentSessionConfiguration : IEntityTypeConfiguration<StudentSession>
{
    public void Configure(EntityTypeBuilder<StudentSession> builder)
    {
        builder.HasIndex(ss => new { ss.StudentId, ss.SessionId }).IsUnique();

        builder.HasOne(ss => ss.Student)
            .WithMany(s => s.StudentSessions)
            .HasForeignKey(ss => ss.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ss => ss.Session)
            .WithMany(s => s.StudentSessions)
            .HasForeignKey(ss => ss.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
