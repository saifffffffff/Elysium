using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyList<Session>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
}
