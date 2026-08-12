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
    /// Get the last error message if operation fails
    /// </summary>
    string? GetLastError();
}
