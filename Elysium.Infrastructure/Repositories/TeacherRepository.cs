using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class TeacherRepository(AppDbContext context) : Repository<Teacher>(context), ITeacherRepository
{
}
