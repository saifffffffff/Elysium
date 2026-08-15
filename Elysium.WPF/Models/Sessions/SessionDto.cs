namespace Elysium.WPF.Models.Sessions;

public record SessionDto(
    int Id,
    string Name,
    string? Description,
    SessionStatus Status,
    DateTime StartedAt,
    DateTime? FinishedAt);