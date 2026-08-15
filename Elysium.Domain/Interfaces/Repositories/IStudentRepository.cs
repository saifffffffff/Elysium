using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByIdWithProfileAsync(int id, CancellationToken cancellationToken = default);
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetAllWithProfileAsync(CancellationToken cancellationToken = default);
}
