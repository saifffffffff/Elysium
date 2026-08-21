namespace Elysium.WPF.Models.Sessions;

public record ChatMessage(
    string Author,
    string Text,
    bool IsUser);