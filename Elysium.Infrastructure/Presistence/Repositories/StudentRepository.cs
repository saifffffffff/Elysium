using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class StudentRepository(AppDbContext context) : Repository<Student>(context), IStudentRepository
{
    public async Task<Student?> GetByIdWithProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Students
            .Include(student => student.User)
            .FirstOrDefaultAsync(student => student.Id == id, cancellationToken);
    }

    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await context.Students
            .Include(student => student.User)
            .FirstOrDefaultAsync(student => student.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetAllWithProfileAsync(CancellationToken cancellationToken = default)
    {
        return await context.Students
            .AsNoTracking()
            .Include(student => student.User)
            .ToListAsync(cancellationToken);
    }
}