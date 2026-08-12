using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class TeacherRepository(AppDbContext context) : Repository<Teacher>(context), ITeacherRepository
{
    public async Task<Teacher?> GetByIdWithProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Teachers
            .Include(teacher => teacher.User)
            .FirstOrDefaultAsync(teacher => teacher.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Teacher>> GetAllWithProfileAsync(CancellationToken cancellationToken = default)
    {
        return await context.Teachers
            .AsNoTracking()
            .Include(teacher => teacher.User)
            .ToListAsync(cancellationToken);
    }
}