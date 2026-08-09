using Elysium.Shared.Models;

namespace Elysium.Shared.Interfaces;

public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyList<Session>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
}
