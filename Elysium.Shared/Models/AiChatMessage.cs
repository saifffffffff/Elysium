using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Shared.Models;

public class AiChatMessage
{
    public int Id { get; set; }
    public int AiChatId { get; set; }
    public string Question { get; set; } = default!;
    public string? Answer { get; set; }
    public DateTime AskedAt { get; set; }
    public DateTime? AnsweredAt { get; set; }

    public AiChat AiChat { get; set; } = default!;

}
