namespace Elysium.WPF.Models;

/// <summary>
/// Request model for user registration/sign up
/// </summary>
public record CreateUserRequest(
    string Username,
    string Password,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    UserRole Role
);
