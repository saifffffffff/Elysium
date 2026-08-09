namespace Elysium.Shared.Models;

public class Session
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public SessionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }    

    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;
    
    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<StudentSession> StudentSessions { get; set; } = new List<StudentSession>();
    public ICollection<TranscriptSegment> TranscriptSegments { get; set; } = new List<TranscriptSegment>();
}
