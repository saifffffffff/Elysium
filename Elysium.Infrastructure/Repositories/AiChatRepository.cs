using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class AiChatRepository(AppDbContext context) : Repository<AiChat>(context), IAiChatRepository
{
}
