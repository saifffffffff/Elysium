using System.Windows;
using Elysium.WPF.Models;

namespace Elysium.WPF.Views;

public partial class TeacherView : Window
{
    public TeacherView()
    {
        InitializeComponent();
        LoadUserInfo();
        LogoutButton.Click += LogoutButton_Click;
    }

    private void LoadUserInfo()
    {
        var currentUser = Application.Current.Resources["CurrentUser"] as AuthResponse;
        if (currentUser != null)
        {
            UserNameTextBlock.Text = $"Welcome, {currentUser.FirstName}!";
            WelcomeTextBlock.Text = $"Hello, {currentUser.FirstName} {currentUser.LastName}!";
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        // Clear user from resources
        Application.Current.Resources.Remove("CurrentUser");

        // Return to sign in
        var signInView = new SignInView(
            (Services.IAuthService)Application.Current.Resources["AuthService"]!,
            (Services.IValidationService)Application.Current.Resources["ValidationService"]!
        );
        signInView.Show();
        Close();
    }
}
