namespace Elysium.WPF.Models;

/// <summary>
/// Profile model returned by the users/{id} endpoint
/// </summary>
public record UserProfile(
    int Id,
    string Username,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    UserRole Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);