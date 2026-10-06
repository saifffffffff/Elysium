namespace Elysium.WPF.Models.Sessions;

public record StartSessionRequest(
    string Name,
    string? Description,
    int CourseId,
    int TeacherId);
