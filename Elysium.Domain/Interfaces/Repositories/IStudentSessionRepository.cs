using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface IStudentSessionRepository : IRepository<StudentSession>
{
    Task<StudentSession?> GetByStudentIdAndSessionId(int sessionId, int studentId, CancellationToken cancellationToken = default);
    Task<bool> IsStudentInSession(int studentId,CancellationToken cancellationToken = default);
}
