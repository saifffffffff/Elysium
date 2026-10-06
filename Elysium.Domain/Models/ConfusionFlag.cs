namespace Elysium.Domain.Models;

public class ConfusionFlag : BaseEntity
{
    public int StudentSessionId { get; set; }
    public DateTime FlaggedAt { get; set; }

    public StudentSession StudentSession { get; set; } = default!;
}
