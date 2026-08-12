namespace Elysium.WPF.Models;

/// <summary>
/// Response model for successful authentication
/// </summary>
public record AuthResponse(
    string Username,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    UserRole Role
);