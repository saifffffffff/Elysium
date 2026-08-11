namespace Elysium.Domain.Models;

public class TranscriptSegment
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public string Text { get; set; } = default!;

    public Session Session { get; set; } = default!;
}
