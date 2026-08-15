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

    /// <summary>
    /// Validate update profile request
    /// </summary>
    List<ValidationError> ValidateUpdateProfile(string firstName, string lastName, DateOnly? birthDate);

    /// <summary>
    /// Validate change username request
    /// </summary>
    List<ValidationError> ValidateChangeUsername(string username);

    /// <summary>
    /// Validate change password request
    /// </summary>
    List<ValidationError> ValidateChangePassword(string currentPassword, string newPassword);

    /// <summary>
    /// Validate create course request (mirrors backend CreateCourseValidator)
    /// </summary>
    List<ValidationError> ValidateCreateCourse(string name, string? description);
}
