using Elysium.Infrastructure.Context;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;

namespace Elysium.Infrastructure.Repositories;

public class TeacherRepository(AppDbContext context) : Repository<Teacher>(context), ITeacherRepository
{
}
