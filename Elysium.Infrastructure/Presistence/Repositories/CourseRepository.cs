using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class CourseRepository(AppDbContext context) : Repository<Course>(context), ICourseRepository
{
    public async Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.Courses.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }
}
