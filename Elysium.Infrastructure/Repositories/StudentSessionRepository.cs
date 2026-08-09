using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class StudentSessionRepository(AppDbContext context) : Repository<StudentSession>(context), IStudentSessionRepository
{
}
