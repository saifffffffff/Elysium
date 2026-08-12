using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Presistence.Repositories;

public class TranscriptSegmentRepository(AppDbContext context) : Repository<TranscriptSegment>(context), ITranscriptSegmentRepository
{
    public async Task<IReadOnlyList<TranscriptSegment>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        return await context.TranscriptSegments.Where(ts => ts.SessionId == sessionId).ToListAsync(cancellationToken);
    }
}
