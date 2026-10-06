using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class AiChatMessageRepository(AppDbContext context) : Repository<AiChatMessage>(context), IAiChatMessageRepository
{
    public async Task<IEnumerable<AiChatMessage>> GetAllBySessionStudentIdAsync(int studentSessionId)
    {
        return await context.AiChatMessages.AsNoTracking().Where(x => x.StudentSessionId == studentSessionId).ToListAsync();
    }
}
