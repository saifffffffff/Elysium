using Elysium.Domain.Models;

namespace Elysium.Domain.Interfaces.Repositories;

public interface ITranscriptSegmentRepository : IRepository<TranscriptSegment>
{
    Task<IReadOnlyList<TranscriptSegment>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
}
