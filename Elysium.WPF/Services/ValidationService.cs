using Elysium.WPF.Models;

namespace Elysium.WPF.Services;

/// <summary>
/// Validation service mirroring backend validation rules
/// </summary>
public class ValidationService : IValidationService
{
    /// <summary>
    /// Validate sign in request
    /// </summary>
    public List<ValidationError> ValidateSignIn(string username, string password)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(username))
            errors.Add(new ValidationError(nameof(username), "Username is required"));

        if (string.IsNullOrWhiteSpace(password))
            errors.Add(new ValidationError(nameof(password), "Password is required"));

        return errors;
    }

    /// <summary>
    /// Validate sign up request (mirrors backend CreateUserRequestValidator)
    /// </summary>
    public List<ValidationError> ValidateSignUp(CreateUserRequest request)
    {
        var errors = new List<ValidationError>();

        // Username validation
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            errors.Add(new ValidationError(nameof(request.Username), "Username is required."));
        }
        else if (request.Username.Length > 64)
        {
            errors.Add(new ValidationError(nameof(request.Username), "Username cannot exceed 64 characters."));
        }

        // Password validation
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add(new ValidationError(nameof(request.Password), "Password is required."));
        }
        else if (request.Password.Length > 128)
        {
            errors.Add(new ValidationError(nameof(request.Password), "Password cannot exceed 128 characters."));
        }

        // First name validation
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            errors.Add(new ValidationError(nameof(request.FirstName), "First name is required."));
        }
        else if (request.FirstName.Length > 64)
        {
            errors.Add(new ValidationError(nameof(request.FirstName), "First name cannot exceed 64 characters."));
        }

        // Last name validation
        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            errors.Add(new ValidationError(nameof(request.LastName), "Last name is required."));
        }
        else if (request.LastName.Length > 64)
        {
            errors.Add(new ValidationError(nameof(request.LastName), "Last name cannot exceed 64 characters."));
        }

        // Birth date validation
        if (request.BirthDate >= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add(new ValidationError(nameof(request.BirthDate), "Birth date must be in the past."));
        }

        return errors;
    }
}
