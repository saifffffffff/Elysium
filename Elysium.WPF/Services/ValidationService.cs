using Elysium.WPF.Models;
using Elysium.WPF.Services.Abstractions;

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

    /// <summary>
    /// Validate update profile request (mirrors backend UpdateProfileRequestValidator)
    /// </summary>
    public List<ValidationError> ValidateUpdateProfile(string firstName, string lastName, DateOnly? birthDate)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(firstName))
        {
            errors.Add(new ValidationError("FirstName", "First name is required."));
        }
        else if (firstName.Length > 64)
        {
            errors.Add(new ValidationError("FirstName", "First name cannot exceed 64 characters."));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            errors.Add(new ValidationError("LastName", "Last name is required."));
        }
        else if (lastName.Length > 64)
        {
            errors.Add(new ValidationError("LastName", "Last name cannot exceed 64 characters."));
        }

        if (birthDate is null)
        {
            errors.Add(new ValidationError("BirthDate", "Birth date is required."));
        }
        else if (birthDate.Value >= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add(new ValidationError("BirthDate", "Birth date must be in the past."));
        }

        return errors;
    }

    /// <summary>
    /// Validate change username request (mirrors backend ChangeUsernameRequestValidator)
    /// </summary>
    public List<ValidationError> ValidateChangeUsername(string username)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(username))
        {
            errors.Add(new ValidationError("Username", "Username is required."));
        }
        else if (username.Length > 64)
        {
            errors.Add(new ValidationError("Username", "Username cannot exceed 64 characters."));
        }

        return errors;
    }

    /// <summary>
    /// Validate change password request (mirrors backend ChangePasswordRequestValidator)
    /// </summary>
    public List<ValidationError> ValidateChangePassword(string currentPassword, string newPassword)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            errors.Add(new ValidationError("CurrentPassword", "Current password is required."));
        }

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            errors.Add(new ValidationError("NewPassword", "New password is required."));
        }
        else if (newPassword.Length > 64)
        {
            errors.Add(new ValidationError("NewPassword", "New password cannot exceed 64 characters."));
        }

        return errors;
    }

    /// <summary>
    /// Validate create course request (mirrors backend CreateCourseValidator)
    /// </summary>
    public List<ValidationError> ValidateCreateCourse(string name, string? description)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new ValidationError("Name", "Course name is required."));
        }
        else if (name.Length > 128)
        {
            errors.Add(new ValidationError("Name", "Course name cannot exceed 128 characters."));
        }

        if (description is not null && description.Length > 512)
        {
            errors.Add(new ValidationError("Description", "Description cannot exceed 512 characters."));
        }

        return errors;
    }

    /// <summary>
    /// Validate create session request (mirrors backend CreateSessionRequestValidator)
    /// </summary>
    public List<ValidationError> ValidateCreateSession(string name, string? description)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new ValidationError("Name", "Name is required."));
        }
        else if (name.Length > 128)
        {
            errors.Add(new ValidationError("Name", "Name cannot exceed 128 characters."));
        }

        if (description is not null && description.Length > 512)
        {
            errors.Add(new ValidationError("Description", "Description cannot exceed 512 characters."));
        }

        return errors;
    }
}
