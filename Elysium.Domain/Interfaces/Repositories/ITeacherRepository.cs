using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByIdWithProfileAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Teacher>> GetAllWithProfileAsync(CancellationToken cancellationToken = default);
}
