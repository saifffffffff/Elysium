using System.Windows;
using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Presenters;

namespace Elysium.WPF.Views;

public partial class StudentView : Window
{
    private StudentPresenter? _coursesPresenter;

    public StudentView()
    {
        InitializeComponent();
        LoadUserInfo();
        LogoutButton.Click += LogoutButton_Click;
        ProfilePanel.BackRequested += (_, _) => ShowDashboardContent();
        ProfilePanel.UserDataChanged += (_, _) => LoadUserInfo();
        CourseDetailsPanel.BackRequested += (_, _) => ShowDashboardContent();

        _coursesPresenter = new StudentPresenter(
            (Services.ICourseService)Application.Current.Resources["CourseService"]!,
            (Services.IEnrollmentService)Application.Current.Resources["EnrollmentService"]!,
            (AuthResponse)Application.Current.Resources["CurrentUser"]!
        );
        _coursesPresenter.CoursesLoaded += Presenter_CoursesLoaded;
        _coursesPresenter.CoursesLoadFailed += Presenter_CoursesLoadFailed;
        _coursesPresenter.EnrollmentSucceeded += Presenter_EnrollmentSucceeded;
        _coursesPresenter.EnrollmentFailed += Presenter_EnrollmentFailed;

        LoadCourses();
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        ProfilePanel.Initialize(
            (Services.IAuthService)Application.Current.Resources["AuthService"]!,
            (Services.IValidationService)Application.Current.Resources["ValidationService"]!,
            (AuthResponse)Application.Current.Resources["CurrentUser"]!
        );

        DashboardContent.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Visible;
        ProfilePanel.LoadProfile();
    }

    private void ShowDashboardContent()
    {
        ProfilePanel.Visibility = Visibility.Collapsed;
        EnrollPanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        DashboardContent.Visibility = Visibility.Visible;
    }

    private void OnCourseCardClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CourseDto course })
        {
            CourseDetailsPanel.Initialize(course);

            DashboardContent.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
            EnrollPanel.Visibility = Visibility.Collapsed;
            CourseDetailsPanel.Visibility = Visibility.Visible;
        }
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

    private async void LoadCourses()
    {
        if (_coursesPresenter is null)
            return;

        CoursesLoadingPanel.Visibility = Visibility.Visible;
        CoursesErrorText.Text = string.Empty;
        CoursesList.Visibility = Visibility.Collapsed;
        EmptyCoursesPanel.Visibility = Visibility.Collapsed;

        await _coursesPresenter.HandleLoadCoursesAsync();
    }

    private void Presenter_CoursesLoaded(object? sender, IReadOnlyList<CourseDto> courses)
    {
        CoursesLoadingPanel.Visibility = Visibility.Collapsed;

        if (courses.Count == 0)
        {
            CoursesList.Visibility = Visibility.Collapsed;
            EmptyCoursesPanel.Visibility = Visibility.Visible;
            return;
        }

        CoursesList.ItemsSource = courses;
        CoursesList.Visibility = Visibility.Visible;
        EmptyCoursesPanel.Visibility = Visibility.Collapsed;
    }

    private void Presenter_CoursesLoadFailed(object? sender, string message)
    {
        CoursesLoadingPanel.Visibility = Visibility.Collapsed;
        CoursesErrorText.Text = message;
    }

    #region Enroll in Course

    private void EnrollCourseButton_Click(object sender, RoutedEventArgs e)
    {
        ResetEnrollForm();

        DashboardContent.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Collapsed;
        EnrollPanel.Visibility = Visibility.Visible;

        CourseCodeInput.Focus();
    }

    private void BackFromEnrollButton_Click(object sender, RoutedEventArgs e)
    {
        ShowDashboardContent();
    }

    private void CancelEnrollButton_Click(object sender, RoutedEventArgs e)
    {
        ShowDashboardContent();
    }

    private void CourseCodeInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter)
            EnrollButton_Click(EnrollButton, e);
    }

    private void ResetEnrollForm()
    {
        CourseCodeInput.Text = string.Empty;
        CourseCodeError.Text = string.Empty;
        EnrollGeneralError.Text = string.Empty;
        EnrollLoadingPanel.Visibility = Visibility.Collapsed;
    }

    private async void EnrollButton_Click(object sender, RoutedEventArgs e)
    {
        if (_coursesPresenter is null)
            return;

        CourseCodeError.Text = string.Empty;
        EnrollGeneralError.Text = string.Empty;

        EnrollButton.IsEnabled = false;
        EnrollLoadingPanel.Visibility = Visibility.Visible;

        try
        {
            await _coursesPresenter.HandleEnrollAsync(CourseCodeInput.Text);
        }
        finally
        {
            EnrollButton.IsEnabled = true;
            EnrollLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void Presenter_EnrollmentSucceeded(object? sender, EventArgs e)
    {
        ResetEnrollForm();
        EnrollPanel.Visibility = Visibility.Collapsed;
        DashboardContent.Visibility = Visibility.Visible;

        LoadCourses();
    }

    private void Presenter_EnrollmentFailed(object? sender, string message)
    {
        if (message == "Please enter a course code.")
        {
            CourseCodeError.Text = message;
            CourseCodeInput.Focus();
        }
        else
        {
            EnrollGeneralError.Text = message;
        }
    }

    #endregion

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
