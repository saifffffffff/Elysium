using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Elysium.Infrastructure.Presistence;

public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<StudentSession> StudentSessions => Set<StudentSession>();
    public DbSet<TranscriptSegment> TranscriptSegments => Set<TranscriptSegment>();
    public DbSet<ConfusionFlag> ConfusionFlags => Set<ConfusionFlag>();
    public DbSet<AiChat> AiChats => Set<AiChat>();
    public DbSet<AiChatMessage> AiChatMessages => Set<AiChatMessage>();

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
