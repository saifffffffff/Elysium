using Elysium.Shared.Models;

namespace Elysium.Shared.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
