using Elysium.WPF.Models;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for sign in view
/// </summary>
public class SignInPresenter
{
    private readonly IAuthService _authService;
    private readonly IValidationService _validationService;

    public event EventHandler? SignInSuccess;
    public event EventHandler<string>? SignInFailed;
    public event EventHandler<List<ValidationError>>? ValidationErrorsChanged;

    private List<ValidationError> _validationErrors = new();
    private AuthResponse? _currentUser;

    public SignInPresenter(IAuthService authService, IValidationService validationService)
    {
        _authService = authService;
        _validationService = validationService;
    }

    /// <summary>
    /// Validate and sign in user
    /// </summary>
    public async Task HandleSignInAsync(string username, string password)
    {
        _validationErrors.Clear();

        // Validate input
        _validationErrors = _validationService.ValidateSignIn(username, password);

        if (_validationErrors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, _validationErrors);
            return;
        }

        try
        {
            var result = await _authService.SignInAsync(username, password);

            if (result != null)
            {
                _currentUser = result;
                ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
                SignInSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                var error = _authService.GetLastError() ?? "Authentication failed";
                SignInFailed?.Invoke(this, error);
            }
        }
        catch (Exception ex)
        {
            SignInFailed?.Invoke(this, ex.Message);
        }
    }

    /// <summary>
    /// Get the authenticated user
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
