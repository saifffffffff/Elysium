using Elysium.WPF.Models;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the profile view
/// </summary>
public class ProfilePresenter
{
    private readonly IAuthService _authService;
    private readonly IValidationService _validationService;
    private AuthResponse _currentUser;
    private UserProfile? _profile;

    public event EventHandler? ProfileLoaded;
    public event EventHandler<string>? ProfileLoadFailed;
    public event EventHandler? ProfileUpdated;
    public event EventHandler<string>? ProfileUpdateFailed;
    public event EventHandler? UsernameChanged;
    public event EventHandler<string>? UsernameChangeFailed;
    public event EventHandler? PasswordChanged;
    public event EventHandler<string>? PasswordChangeFailed;
    public event EventHandler<List<ValidationError>>? ValidationErrorsChanged;

    public ProfilePresenter(IAuthService authService, IValidationService validationService, AuthResponse currentUser)
    {
        _authService = authService;
        _validationService = validationService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// The id of the current user
    /// </summary>
    public int UserId => _currentUser.Id;

    /// <summary>
    /// The latest profile loaded from the server
    /// </summary>
    public UserProfile? Profile => _profile;

    /// <summary>
    /// The current user as known by the presenter
    /// </summary>
    public AuthResponse CurrentUser => _currentUser;

    /// <summary>
    /// Load the profile of the current user from the server
    /// </summary>
    public async Task HandleLoadProfileAsync()
    {
        _profile = await _authService.GetProfileAsync(UserId);

        if (_profile is null)
        {
            ProfileLoadFailed?.Invoke(this, _authService.GetLastError() ?? "Failed to load profile.");
            return;
        }

        _currentUser = _currentUser with
        {
            Username = _profile.Username,
            FirstName = _profile.FirstName,
            LastName = _profile.LastName,
            BirthDate = _profile.BirthDate
        };

        ProfileLoaded?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Validate and update the profile information
    /// </summary>
    public async Task HandleUpdateProfileAsync(string firstName, string lastName, DateOnly? birthDate)
    {
        var errors = _validationService.ValidateUpdateProfile(firstName, lastName, birthDate);

        if (errors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, errors);
            return;
        }

        var request = new UpdateProfileRequest(UserId, firstName.Trim(), lastName.Trim(), birthDate!.Value);

        if (!await _authService.UpdateProfileAsync(request))
        {
            ProfileUpdateFailed?.Invoke(this, _authService.GetLastError() ?? "Failed to update profile.");
            return;
        }

        _currentUser = _currentUser with { FirstName = firstName.Trim(), LastName = lastName.Trim(), BirthDate = birthDate.Value };

        if (_profile is not null)
            _profile = _profile with { FirstName = firstName.Trim(), LastName = lastName.Trim(), BirthDate = birthDate.Value };

        ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
        ProfileUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Validate and change the username
    /// </summary>
    public async Task HandleChangeUsernameAsync(string username)
    {
        var errors = _validationService.ValidateChangeUsername(username);

        if (errors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, errors);
            return;
        }

        var request = new ChangeUsernameRequest(UserId, username.Trim());

        if (!await _authService.ChangeUsernameAsync(request))
        {
            UsernameChangeFailed?.Invoke(this, _authService.GetLastError() ?? "Failed to change username.");
            return;
        }

        _currentUser = _currentUser with { Username = username.Trim() };

        if (_profile is not null)
            _profile = _profile with { Username = username.Trim() };

        ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
        UsernameChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Validate and change the password
    /// </summary>
    public async Task HandleChangePasswordAsync(string currentPassword, string newPassword)
    {
        var errors = _validationService.ValidateChangePassword(currentPassword, newPassword);

        if (errors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, errors);
            return;
        }

        var request = new ChangePasswordRequest(UserId, currentPassword, newPassword);

        if (!await _authService.ChangePasswordAsync(request))
        {
            PasswordChangeFailed?.Invoke(this, _authService.GetLastError() ?? "Failed to change password.");
            return;
        }

        ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
        PasswordChanged?.Invoke(this, EventArgs.Empty);
    }
}