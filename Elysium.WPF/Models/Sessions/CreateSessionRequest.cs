namespace Elysium.WPF.Models.Sessions;

public record CreateSessionRequest(string Name, string? Description, int CourseId);
