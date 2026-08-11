using Elysium.Infrastructure.Context;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;

namespace Elysium.Infrastructure.Repositories;

public class MaterialRepository(AppDbContext context) : Repository<Material>(context), IMaterialRepository
{
}
