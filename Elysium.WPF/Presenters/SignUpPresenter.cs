using Elysium.WPF.Models;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for sign up view
/// </summary>
public class SignUpPresenter
{
    private readonly IAuthService _authService;
    private readonly IValidationService _validationService;

    public event EventHandler? SignUpSuccess;
    public event EventHandler<string>? SignUpFailed;
    public event EventHandler<List<ValidationError>>? ValidationErrorsChanged;

    private List<ValidationError> _validationErrors = new();
    private AuthResponse? _currentUser;

    public SignUpPresenter(IAuthService authService, IValidationService validationService)
    {
        _authService = authService;
        _validationService = validationService;
    }

    /// <summary>
    /// Validate and register new user
    /// </summary>
    public async Task HandleSignUpAsync(CreateUserRequest request)
    {
        _validationErrors.Clear();

        // Validate input
        _validationErrors = _validationService.ValidateSignUp(request);

        if (_validationErrors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, _validationErrors);
            return;
        }

        try
        {
            var result = await _authService.SignUpAsync(request);

            if (result != null)
            {
                // Sign-up endpoint only returns the new user's id, so re-authenticate
                // to obtain the full profile and the correct role.
                var authenticatedUser = await _authService.SignInAsync(request.Username, request.Password);

                if (authenticatedUser != null)
                {
                    _currentUser = authenticatedUser;
                    ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
                    SignUpSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }

                SignUpFailed?.Invoke(this, "Account created. Please sign in.");
                return;
            }

            var error = _authService.GetLastError() ?? "Registration failed";
            SignUpFailed?.Invoke(this, error);
        }
        catch (Exception ex)
        {
            SignUpFailed?.Invoke(this, ex.Message);
        }
    }

    /// <summary>
    /// Get the registered user
    /// </summary>
    public AuthResponse? GetCurrentUser()
    {
        return _currentUser;
    }

    /// <summary>
    /// Get current validation errors
    /// </summary>
    public List<ValidationError> GetValidationErrors()
    {
        return _validationErrors;
    }
}
