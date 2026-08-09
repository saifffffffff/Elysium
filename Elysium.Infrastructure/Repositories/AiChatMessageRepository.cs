using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;

namespace Elysium.Infrastructure.Repositories;

public class AiChatMessageRepository(AppDbContext context) : Repository<AiChatMessage>(context), IAiChatMessageRepository
{
}
