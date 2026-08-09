using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class MaterialRepository(AppDbContext context) : Repository<Material>(context), IMaterialRepository
{
}
