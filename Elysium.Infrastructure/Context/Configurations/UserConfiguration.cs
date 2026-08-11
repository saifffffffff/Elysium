using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elysium.Infrastructure.Context.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnType("VARCHAR");
        
        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Role)
            .HasColumnType("Tinyint");

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
            

        builder.Property(x => x.BirthDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        

        builder.ToTable("Users", t => {
            t.HasCheckConstraint("CK_Users_BirthDate", "(BirthDate < CAST(GETDATE() AS DATE))");
            t.HasCheckConstraint("CK_Users_Role", "Role IN (0 , 1 )");
            });
    }
}
