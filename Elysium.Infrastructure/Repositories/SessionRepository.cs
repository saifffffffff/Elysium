using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Repositories;

public class SessionRepository(AppDbContext context) : Repository<Session>(context), ISessionRepository
{
    public async Task<IReadOnlyList<Session>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await context.Sessions.Where(s => s.CourseId == courseId).ToListAsync(cancellationToken);
    }
}
