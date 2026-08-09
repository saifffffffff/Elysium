using Elysium.Shared.Models;

namespace Elysium.Shared.Interfaces;

public interface ITranscriptSegmentRepository : IRepository<TranscriptSegment>
{
    Task<IReadOnlyList<TranscriptSegment>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
}
