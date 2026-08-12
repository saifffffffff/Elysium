using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Presistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasIndex(s => s.UserId).IsUnique();

        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
