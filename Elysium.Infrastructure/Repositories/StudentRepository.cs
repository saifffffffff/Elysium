using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class StudentRepository(AppDbContext context) : Repository<Student>(context), IStudentRepository
{
}
