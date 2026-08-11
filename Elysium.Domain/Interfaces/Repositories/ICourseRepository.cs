using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
