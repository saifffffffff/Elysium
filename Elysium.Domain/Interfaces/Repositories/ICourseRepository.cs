using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdWithSessionsAsync(int Id, CancellationToken cancellationToken = default);
}
