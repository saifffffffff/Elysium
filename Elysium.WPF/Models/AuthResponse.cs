namespace Elysium.WPF.Models;

/// <summary>
/// Response model for successful authentication
/// </summary>
public record AuthResponse(
    int Id,
    string Username,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    UserRole Role,
    int? TeacherId,
    int? StudentId
);