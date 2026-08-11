using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {


        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(512);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(6);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");


        builder.ToTable("Courses", t =>
        {
            t.HasCheckConstraint("CK_Courses_Name", "LEN(TRIM(Name)) > 0");
            t.HasCheckConstraint("CK_Courses_Code", "LEN(TRIM(Code)) = 6");
        });


        builder.HasOne(x => x.Teacher).WithMany(x => x.Courses).HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Members).WithMany(x => x.Courses).UsingEntity<Enrollment>();
    }
}
