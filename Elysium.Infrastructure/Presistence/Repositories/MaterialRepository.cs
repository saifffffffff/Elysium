using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class MaterialRepository(AppDbContext context) : Repository<Material>(context), IMaterialRepository
{
}
