using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class StudentSessionRepository(AppDbContext context) : Repository<StudentSession>(context), IStudentSessionRepository
{
}
