using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class AiChatRepository(AppDbContext context) : Repository<AiChat>(context), IAiChatRepository
{
}
