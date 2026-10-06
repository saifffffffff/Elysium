using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class StudentSessionRepository(AppDbContext context) : Repository<StudentSession>(context), IStudentSessionRepository
{
    public async Task<StudentSession?> GetByStudentIdAndSessionId(int sessionId , int studentId , CancellationToken cancellationToken = default)
    {
        return await context.StudentSessions
            .FirstOrDefaultAsync(ss => ss.StudentId == studentId && ss.SessionId == sessionId , cancellationToken);
    }

    public async Task<bool> IsStudentInSession(int studentId ,CancellationToken cancellationToken = default)
    {
        return await context.StudentSessions
            .AsNoTracking()
            .AnyAsync(ss => ss.StudentId == studentId && ss.IsInSession);
    }
}
