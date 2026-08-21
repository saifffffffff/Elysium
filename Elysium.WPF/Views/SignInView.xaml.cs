using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Elysium.WPF.Models;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Views;

public partial class SignInView : Window
{
    private readonly SignInPresenter _signInPresenter;
    private readonly SignUpPresenter _signUpPresenter;

    private const double SlideOffset = 380;
    private readonly TimeSpan _animDuration = TimeSpan.FromSeconds(0.32);
    private bool _inSignUp;

    public SignInView(IAuthService authService, IValidationService validationService)
    {
        InitializeComponent();

        _signInPresenter = new SignInPresenter(authService, validationService);
        _signUpPresenter = new SignUpPresenter(authService, validationService);

        // Sign-in wiring
        SignInButton.Click += SignInButton_Click;
        SignInUsernameTextBox.KeyDown += Credential_KeyDown;
        SignInPasswordBox.KeyDown += Credential_KeyDown;
        SignUpLink.Click += SwitchToSignUp_Click;
        _signInPresenter.SignInSuccess += Presenter_SignInSuccess;
        _signInPresenter.SignInFailed += Presenter_SignInFailed;
        _signInPresenter.ValidationErrorsChanged += SignInPresenter_ValidationErrorsChanged;

        // Sign-up wiring
        SignUpButton.Click += SignUpButton_Click;
        SignUpUsernameTextBox.KeyDown += Credential_KeyDown;
        SignUpPasswordBox.KeyDown += Credential_KeyDown;
        SignUpFirstNameBox.KeyDown += Credential_KeyDown;
        SignUpLastNameBox.KeyDown += Credential_KeyDown;
        SignInLink.Click += SwitchToSignIn_Click;
        _signUpPresenter.SignUpSuccess += Presenter_SignUpSuccess;
        _signUpPresenter.SignUpFailed += Presenter_SignUpFailed;
        _signUpPresenter.ValidationErrorsChanged += SignUpPresenter_ValidationErrorsChanged;

        RestoreRememberedCredentials();
    }

    private void RestoreRememberedCredentials()
    {
        var saved = CredentialStore.Load();
        if (saved is not null)
        {
            SignInUsernameTextBox.Text = saved.Value.Username;
            SignInPasswordBox.Password = saved.Value.Password;
            RememberMeCheckBox.IsChecked = true;
        }
    }

    #region Custom chrome

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    #endregion

    #region Slide animation between Sign In / Sign Up

    private void SwitchToSignUp_Click(object sender, RoutedEventArgs e)
    {
        Navigate(toSignUp: true);
    }

    private void SwitchToSignIn_Click(object sender, RoutedEventArgs e)
    {
        Navigate(toSignUp: false);
    }

    private void Navigate(bool toSignUp)
    {
        if (toSignUp == _inSignUp)
            return;

        var signInTx = (TranslateTransform)SignInPanel.RenderTransform;
        var signUpTx = (TranslateTransform)SignUpPanel.RenderTransform;

        var fadeIn = new DoubleAnimation { To = 1, Duration = _animDuration, DecelerationRatio = 0.4 };
        var fadeOut = new DoubleAnimation { To = 0, Duration = _animDuration, AccelerationRatio = 0.4 };

        if (toSignUp)
        {
            HeaderTitle.Text = "Join Elysium";
            HeaderSubtitle.Text = "Create your account and start your learning journey today.";

            // sign-in slides out to the left
            SignInPanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            signInTx.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation { To = -SlideOffset, Duration = _animDuration, AccelerationRatio = 0.4 });

            // sign-up slides in from the right
            signUpTx.X = SlideOffset;
            SignUpPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            signUpTx.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation { To = 0, Duration = _animDuration, DecelerationRatio = 0.4 });
        }
        else
        {
            HeaderTitle.Text = "Welcome Back";
            HeaderSubtitle.Text = "Sign in to your account to continue learning and teaching.";

            // sign-up slides out to the right
            SignUpPanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            signUpTx.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation { To = SlideOffset, Duration = _animDuration, AccelerationRatio = 0.4 });

            // sign-in slides in from the left
            signInTx.X = -SlideOffset;
            SignInPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            signInTx.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation { To = 0, Duration = _animDuration, DecelerationRatio = 0.4 });
        }

        _inSignUp = toSignUp;
    }

    private void Credential_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (_inSignUp)
                SignUpButton_Click(SignUpButton, e);
            else
                SignInButton_Click(SignInButton, e);
        }
    }

    #endregion

    #region Sign-in actions

    private async void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        SignInButton.IsEnabled = false;
        SignInLoadingPanel.Visibility = Visibility.Visible;
        SignInGeneralError.Text = "";

        try
        {
            await _signInPresenter.HandleSignInAsync(SignInUsernameTextBox.Text, SignInPasswordBox.Password);
        }
        finally
        {
            SignInButton.IsEnabled = true;
            SignInLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void Presenter_SignInSuccess(object? sender, EventArgs e)
    {
        if (RememberMeCheckBox.IsChecked == true)
            CredentialStore.Save(SignInUsernameTextBox.Text, SignInPasswordBox.Password);
        else
            CredentialStore.Delete();

        var user = _signInPresenter.GetCurrentUser();
        if (user != null)
        {
            Application.Current.Resources["CurrentUser"] = user;

            Window dashboard = user.Role == UserRole.Student
                ? (Window)new StudentView()
                : (Window)new TeacherView();

            dashboard.Show();
            Close();
        }
    }

    private void Presenter_SignInFailed(object? sender, string error)
    {
        SignInGeneralError.Text = error;
    }

    private void SignInPresenter_ValidationErrorsChanged(object? sender, List<ValidationError> errors)
    {
        SignInUsernameError.Text = "";
        SignInPasswordError.Text = "";

        foreach (var error in errors)
        {
            switch (error.Field.ToLower())
            {
                case "username":
                    SignInUsernameError.Text = error.Message;
                    break;
                case "password":
                    SignInPasswordError.Text = error.Message;
                    break;
            }
        }
    }

    #endregion

    #region Sign-up actions

    private async void SignUpButton_Click(object sender, RoutedEventArgs e)
    {
        SignUpButton.IsEnabled = false;
        SignUpLoadingPanel.Visibility = Visibility.Visible;
        SignUpGeneralError.Text = "";

        try
        {
            if (BirthDatePicker.SelectedDate == null)
            {
                SignUpGeneralError.Text = "Birth date is required.";
                return;
            }

            var request = new CreateUserRequest(
                SignUpUsernameTextBox.Text,
                SignUpPasswordBox.Password,
                SignUpFirstNameBox.Text,
                SignUpLastNameBox.Text,
                DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value),
                StudentRadio.IsChecked == true ? UserRole.Student : UserRole.Teacher
            );

            await _signUpPresenter.HandleSignUpAsync(request);
        }
        finally
        {
            SignUpButton.IsEnabled = true;
            SignUpLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void Presenter_SignUpSuccess(object? sender, EventArgs e)
    {
        var user = _signUpPresenter.GetCurrentUser();
        if (user != null)
        {
            Application.Current.Resources["CurrentUser"] = user;

            Window dashboard = user.Role == UserRole.Student
                ? (Window)new StudentView()
                : (Window)new TeacherView();

            dashboard.Show();
            Close();
        }
    }

    private void Presenter_SignUpFailed(object? sender, string error)
    {
        SignUpGeneralError.Text = error;
    }

    private void SignUpPresenter_ValidationErrorsChanged(object? sender, List<ValidationError> errors)
    {
        SignUpUsernameError.Text = "";
        SignUpPasswordError.Text = "";
        SignUpFirstNameError.Text = "";
        SignUpLastNameError.Text = "";
        SignUpBirthError.Text = "";

        foreach (var error in errors)
        {
            switch (error.Field.ToLower())
            {
                case "username":
                    SignUpUsernameError.Text = error.Message;
                    break;
                case "password":
                    SignUpPasswordError.Text = error.Message;
                    break;
                case "firstname":
                    SignUpFirstNameError.Text = error.Message;
                    break;
                case "lastname":
                    SignUpLastNameError.Text = error.Message;
                    break;
                case "birthdate":
                    SignUpBirthError.Text = error.Message;
                    break;
            }
        }
    }

    #endregion
}
