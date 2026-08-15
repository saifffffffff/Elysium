using System.Windows;
using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Presenters;

namespace Elysium.WPF.Views;

public partial class TeacherView : Window
{
    private TeacherCoursesPresenter? _coursesPresenter;

    public TeacherView()
    {
        InitializeComponent();
        LoadUserInfo();
        LogoutButton.Click += LogoutButton_Click;
        ProfilePanel.BackRequested += (_, _) => ShowDashboardContent();
        ProfilePanel.UserDataChanged += (_, _) => LoadUserInfo();
        CoursePanel.BackRequested += (_, _) => ShowDashboardContent();
        CourseDetailsPanel.BackRequested += (_, _) => ShowDashboardContent();

        _coursesPresenter = new TeacherCoursesPresenter(
            (Services.ICourseService)Application.Current.Resources["CourseService"]!,
            (Services.IValidationService)Application.Current.Resources["ValidationService"]!,
            (AuthResponse)Application.Current.Resources["CurrentUser"]!
        );
        _coursesPresenter.CoursesLoaded += Presenter_CoursesLoaded;
        _coursesPresenter.CoursesLoadFailed += Presenter_CoursesLoadFailed;
        _coursesPresenter.CourseCreated += Presenter_CourseCreated;

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
        CoursePanel.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Visible;
        ProfilePanel.LoadProfile();
    }

    private void CreateCourseButton_Click(object sender, RoutedEventArgs e)
    {
        CoursePanel.Initialize(_coursesPresenter!);
        CoursePanel.Reset();

        DashboardContent.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Collapsed;
        CoursePanel.Visibility = Visibility.Visible;
    }

    private void ShowDashboardContent()
    {
        ProfilePanel.Visibility = Visibility.Collapsed;
        CoursePanel.Visibility = Visibility.Collapsed;
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
            CoursePanel.Visibility = Visibility.Collapsed;
            CourseDetailsPanel.Visibility = Visibility.Visible;
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

    private void Presenter_CourseCreated(object? sender, CreateCourseResponse response)
    {
        LoadCourses();
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
