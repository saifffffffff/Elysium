namespace Elysium.Domain.Models;

public class StudentSession
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SessionId { get; set; }
    public DateTime JoinedAt { get; set; }

    public Student Student { get; set; } = default!;
    public Session Session { get; set; } = default!;
    public ICollection<ConfusionFlag> ConfusionFlags { get; set; } = new List<ConfusionFlag>();
    public ICollection<AiChat> AiChats { get; set; } = new List<AiChat>();
}
