using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(x => new {x.StudentId , x.CourseId});
        
        builder.Property(x => x.EnrollmentDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

    }
}
