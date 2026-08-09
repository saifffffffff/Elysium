using Elysium.Infrastructure.Context;
using Elysium.Shared.Interfaces;
using Elysium.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Repositories;

public class TranscriptSegmentRepository(AppDbContext context) : Repository<TranscriptSegment>(context), ITranscriptSegmentRepository
{
    public async Task<IReadOnlyList<TranscriptSegment>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        return await context.TranscriptSegments.Where(ts => ts.SessionId == sessionId).ToListAsync(cancellationToken);
    }
}
