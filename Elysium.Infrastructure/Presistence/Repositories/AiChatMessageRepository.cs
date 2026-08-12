using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class AiChatMessageRepository(AppDbContext context) : Repository<AiChatMessage>(context), IAiChatMessageRepository
{
}
