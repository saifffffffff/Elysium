namespace Elysium.Shared.Models;

public class AiChat
{
    public int Id { get; set; }
    public int StudentSessionId { get; set; }
    
    public StudentSession StudentSession { get; set; } = default!;
    public ICollection<AiChatMessage> Messages { get; set; } = new List<AiChatMessage>();
}
