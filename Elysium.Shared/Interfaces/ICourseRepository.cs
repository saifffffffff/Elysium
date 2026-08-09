using Elysium.Shared.Models;

namespace Elysium.Shared.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
