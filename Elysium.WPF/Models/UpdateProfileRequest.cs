namespace Elysium.WPF.Models;

/// <summary>
/// Request model for updating a user profile
/// </summary>
public record UpdateProfileRequest(
    int Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate
);