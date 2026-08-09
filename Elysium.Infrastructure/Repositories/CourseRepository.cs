using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Repositories;

public class CourseRepository(AppDbContext context) : Repository<Course>(context), ICourseRepository
{
    public async Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.Courses.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }
}
