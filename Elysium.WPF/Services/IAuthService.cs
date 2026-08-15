namespace Elysium.WPF.Services;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate user with username and password
    /// </summary>
    Task<Models.AuthResponse?> SignInAsync(string username, string password);

    /// <summary>
    /// Register a new user
    /// </summary>
    Task<Models.AuthResponse?> SignUpAsync(Models.CreateUserRequest request);

    /// <summary>
    /// Get the profile for a user by id
    /// </summary>
    Task<Models.UserProfile?> GetProfileAsync(int id);

    /// <summary>
    /// Update the profile information of the current user
    /// </summary>
    Task<bool> UpdateProfileAsync(Models.UpdateProfileRequest request);

    /// <summary>
    /// Change the username of the current user
    /// </summary>
    Task<bool> ChangeUsernameAsync(Models.ChangeUsernameRequest request);

    /// <summary>
    /// Change the password of the current user
    /// </summary>
    Task<bool> ChangePasswordAsync(Models.ChangePasswordRequest request);

    /// <summary>
    /// Get the last error message if operation fails
    /// </summary>
    string? GetLastError();
}
