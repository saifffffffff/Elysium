using Elysium.WPF.Models;

namespace Elysium.WPF.Services;

/// <summary>
/// Interface for validation service
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validate sign in request
    /// </summary>
    List<ValidationError> ValidateSignIn(string username, string password);

    /// <summary>
    /// Validate sign up request
    /// </summary>
    List<ValidationError> ValidateSignUp(CreateUserRequest request);
}
