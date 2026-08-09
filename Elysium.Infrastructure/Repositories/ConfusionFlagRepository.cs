using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class ConfusionFlagRepository(AppDbContext context) : Repository<ConfusionFlag>(context), IConfusionFlagRepository
{
}
