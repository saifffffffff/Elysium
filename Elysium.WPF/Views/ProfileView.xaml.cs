using System.Windows;
using System.Windows.Controls;
using Elysium.WPF.Models;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;
using ValidationError = Elysium.WPF.Models.ValidationError;

namespace Elysium.WPF.Views;

/// <summary>
/// Interaction logic for ProfileView
/// </summary>
public partial class ProfileView : UserControl
{
    private ProfilePresenter? _presenter;

    /// <summary>
    /// Raised when the user requests to go back to the dashboard
    /// </summary>
    public event EventHandler? BackRequested;

    /// <summary>
    /// Raised when the stored user data changed (username / profile)
    /// </summary>
    public event EventHandler? UserDataChanged;

    public ProfileView()
    {
        InitializeComponent();
        BackButton.Click += BackButton_Click;
    }

    /// <summary>
    /// Wire the services and the current user; safe to call more than once
    /// </summary>
    public void Initialize(IAuthService authService, IValidationService validationService, AuthResponse currentUser)
    {
        if (_presenter is not null)
            return;

        _presenter = new ProfilePresenter(authService, validationService, currentUser);

        _presenter.ProfileLoaded += Presenter_ProfileLoaded;
        _presenter.ProfileLoadFailed += Presenter_ProfileLoadFailed;
        _presenter.ProfileUpdated += Presenter_ProfileUpdated;
        _presenter.ProfileUpdateFailed += Presenter_ProfileUpdateFailed;
        _presenter.UsernameChanged += Presenter_UsernameChanged;
        _presenter.UsernameChangeFailed += Presenter_UsernameChangeFailed;
        _presenter.PasswordChanged += Presenter_PasswordChanged;
        _presenter.PasswordChangeFailed += Presenter_PasswordChangeFailed;
        _presenter.ValidationErrorsChanged += Presenter_ValidationErrorsChanged;

        UpdateButton.Click += UpdateButton_Click;
        UsernameButton.Click += UsernameButton_Click;
        PasswordButton.Click += PasswordButton_Click;

        ShowProfile(_presenter.CurrentUser);
    }

    /// <summary>
    /// Load the profile of the current user from the server
    /// </summary>
    public async void LoadProfile()
    {
        if (_presenter is null)
            return;

        OverviewLoadingPanel.Visibility = Visibility.Visible;
        OverviewErrorText.Text = string.Empty;
        await _presenter.HandleLoadProfileAsync();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ShowProfile(AuthResponse user)
    {
        InitialsText.Text = GetInitials(user.FirstName, user.LastName);
        UsernameText.Text = user.Username;
        RoleText.Text = user.Role == UserRole.Teacher ? "Teacher" : "Student";
        FirstNameDisplay.Text = user.FirstName;
        LastNameDisplay.Text = user.LastName;
        BirthDateDisplay.Text = user.BirthDate.ToString("dd MMM yyyy");

        FirstNameInput.Text = user.FirstName;
        LastNameInput.Text = user.LastName;
        UpdateBirthDatePicker.SelectedDate = user.BirthDate.ToDateTime(TimeOnly.MinValue);
    }

    private void ShowProfile(UserProfile profile)
    {
        InitialsText.Text = GetInitials(profile.FirstName, profile.LastName);
        UsernameText.Text = profile.Username;
        RoleText.Text = profile.Role == UserRole.Teacher ? "Teacher" : "Student";
        FirstNameDisplay.Text = profile.FirstName;
        LastNameDisplay.Text = profile.LastName;
        BirthDateDisplay.Text = profile.BirthDate.ToString("dd MMM yyyy");
        MemberSinceDisplay.Text = profile.CreatedAt.ToString("dd MMM yyyy");
    }

    private static string GetInitials(string firstName, string lastName)
    {
        string f = string.IsNullOrWhiteSpace(firstName) ? string.Empty : firstName.Trim().Substring(0, 1);
        string l = string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName.Trim().Substring(0, 1);
        return (f + l).ToUpperInvariant();
    }

    private void UpdateStoredUser()
    {
        if (_presenter is null)
            return;

        Application.Current.Resources["CurrentUser"] = _presenter.CurrentUser;
    }

    #region Presenter events

    private void Presenter_ProfileLoaded(object? sender, EventArgs e)
    {
        OverviewLoadingPanel.Visibility = Visibility.Collapsed;

        if (_presenter!.Profile is UserProfile profile)
        {
            ShowProfile(profile);
            UpdateStoredUser();
            UserDataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Presenter_ProfileLoadFailed(object? sender, string message)
    {
        OverviewLoadingPanel.Visibility = Visibility.Collapsed;
        OverviewErrorText.Text = message;
    }

    private void Presenter_ProfileUpdated(object? sender, EventArgs e)
    {
        UpdateLoadingPanel.Visibility = Visibility.Collapsed;
        UpdateGeneralError.Text = string.Empty;
        UpdateSuccessText.Text = "Profile updated successfully.";

        ShowProfile(_presenter!.CurrentUser);
        UpdateStoredUser();
        UserDataChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Presenter_ProfileUpdateFailed(object? sender, string message)
    {
        UpdateLoadingPanel.Visibility = Visibility.Collapsed;
        UpdateSuccessText.Text = string.Empty;
        UpdateGeneralError.Text = message;
    }

    private void Presenter_UsernameChanged(object? sender, EventArgs e)
    {
        UsernameLoadingPanel.Visibility = Visibility.Collapsed;
        UsernameGeneralError.Text = string.Empty;
        UsernameSuccessText.Text = "Username updated successfully.";
        UsernameInput.Text = string.Empty;

        UsernameText.Text = _presenter!.CurrentUser.Username;
        UpdateStoredUser();
        UserDataChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Presenter_UsernameChangeFailed(object? sender, string message)
    {
        UsernameLoadingPanel.Visibility = Visibility.Collapsed;
        UsernameSuccessText.Text = string.Empty;
        UsernameGeneralError.Text = message;
    }

    private void Presenter_PasswordChanged(object? sender, EventArgs e)
    {
        PasswordLoadingPanel.Visibility = Visibility.Collapsed;
        PasswordGeneralError.Text = string.Empty;
        PasswordSuccessText.Text = "Password updated successfully.";
        CurrentPasswordBox.Password = string.Empty;
        NewPasswordBox.Password = string.Empty;
    }

    private void Presenter_PasswordChangeFailed(object? sender, string message)
    {
        PasswordLoadingPanel.Visibility = Visibility.Collapsed;
        PasswordSuccessText.Text = string.Empty;
        PasswordGeneralError.Text = message;
    }

    private void Presenter_ValidationErrorsChanged(object? sender, List<ValidationError> errors)
    {
        FirstNameError.Text = string.Empty;
        LastNameError.Text = string.Empty;
        BirthDateError.Text = string.Empty;
        UsernameError.Text = string.Empty;
        CurrentPasswordError.Text = string.Empty;
        NewPasswordError.Text = string.Empty;
        UpdateGeneralError.Text = string.Empty;
        UsernameGeneralError.Text = string.Empty;
        PasswordGeneralError.Text = string.Empty;
        UpdateSuccessText.Text = string.Empty;
        UsernameSuccessText.Text = string.Empty;
        PasswordSuccessText.Text = string.Empty;

        foreach (var error in errors)
        {
            switch (error.Field.ToLower())
            {
                case "firstname":
                    FirstNameError.Text = error.Message;
                    break;
                case "lastname":
                    LastNameError.Text = error.Message;
                    break;
                case "birthdate":
                    BirthDateError.Text = error.Message;
                    break;
                case "username":
                    UsernameError.Text = error.Message;
                    break;
                case "currentpassword":
                    CurrentPasswordError.Text = error.Message;
                    break;
                case "newpassword":
                    NewPasswordError.Text = error.Message;
                    break;
            }
        }
    }

    #endregion

    #region Actions

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        if (_presenter is null)
            return;

        UpdateButton.IsEnabled = false;
        UpdateLoadingPanel.Visibility = Visibility.Visible;
        UpdateSuccessText.Text = string.Empty;

        try
        {
            var birthDate = UpdateBirthDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(UpdateBirthDatePicker.SelectedDate.Value)
                : (DateOnly?)null;

            await _presenter.HandleUpdateProfileAsync(FirstNameInput.Text, LastNameInput.Text, birthDate);
        }
        finally
        {
            UpdateButton.IsEnabled = true;
            UpdateLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    private async void UsernameButton_Click(object sender, RoutedEventArgs e)
    {
        if (_presenter is null)
            return;

        UsernameButton.IsEnabled = false;
        UsernameLoadingPanel.Visibility = Visibility.Visible;
        UsernameSuccessText.Text = string.Empty;

        try
        {
            await _presenter.HandleChangeUsernameAsync(UsernameInput.Text);
        }
        finally
        {
            UsernameButton.IsEnabled = true;
            UsernameLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    private async void PasswordButton_Click(object sender, RoutedEventArgs e)
    {
        if (_presenter is null)
            return;

        PasswordButton.IsEnabled = false;
        PasswordLoadingPanel.Visibility = Visibility.Visible;
        PasswordSuccessText.Text = string.Empty;

        try
        {
            await _presenter.HandleChangePasswordAsync(CurrentPasswordBox.Password, NewPasswordBox.Password);
        }
        finally
        {
            PasswordButton.IsEnabled = true;
            PasswordLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    #endregion
}