namespace Elysium.Shared.Models;

public class ConfusionFlag
{
    public int Id { get; set; }
    public int StudentSessionId { get; set; }
    public DateTime FlaggedAt { get; set; }

    public StudentSession StudentSession { get; set; } = default!;
}
