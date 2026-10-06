using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface IAiChatMessageRepository : IRepository<AiChatMessage>
{
    Task<IEnumerable<AiChatMessage>> GetAllBySessionStudentIdAsync(int studentSessionId);
}
