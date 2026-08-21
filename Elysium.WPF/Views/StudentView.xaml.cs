using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Views;

public partial class StudentView : Window
{
    private StudentPresenter? _coursesPresenter;
    private ISessionHubService? _sessionHubService;
    private int? _activeSessionId;
    private DispatcherTimer? _toastTimer;

    public StudentView()
    {
        InitializeComponent();
        LoadUserInfo();
        LogoutButton.Click += LogoutButton_Click;
        Closed += StudentView_Closed;
        ProfilePanel.BackRequested += (_, _) => ShowDashboardContent();
        ProfilePanel.UserDataChanged += (_, _) => LoadUserInfo();
        CourseDetailsPanel.BackRequested += (_, _) => ShowDashboardContent();
        CourseDetailsPanel.SessionJoinRequested += OnSessionJoinRequested;
        SessionPanel.LeaveRequested += OnLeaveSessionRequested;
        SessionHubService.SessionEnded += OnSessionEnded;
        SessionHubService.TranscriptAppended += OnTranscriptAppended;

        _coursesPresenter = new StudentPresenter(
            (ICourseService)Application.Current.Resources["CourseService"]!,
            (IEnrollmentService)Application.Current.Resources["EnrollmentService"]!,
            (AuthResponse)Application.Current.Resources["CurrentUser"]!
        );
        _coursesPresenter.CoursesLoaded += Presenter_CoursesLoaded;
        _coursesPresenter.CoursesLoadFailed += Presenter_CoursesLoadFailed;
        _coursesPresenter.EnrollmentSucceeded += Presenter_EnrollmentSucceeded;
        _coursesPresenter.EnrollmentFailed += Presenter_EnrollmentFailed;

        LoadCourses();
    }

    private ISessionHubService SessionHubService =>
        _sessionHubService ??= (ISessionHubService)Application.Current.Resources["SessionHubService"]!;

    #region Custom chrome

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    #endregion

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        ProfilePanel.Initialize(
            (IAuthService)Application.Current.Resources["AuthService"]!,
            (IValidationService)Application.Current.Resources["ValidationService"]!,
            (AuthResponse)Application.Current.Resources["CurrentUser"]!
        );

        DashboardContent.Visibility = Visibility.Collapsed;
        EnrollPanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Visible;
        ProfilePanel.LoadProfile();
    }

    private async void ShowDashboardContent()
    {
        await CourseDetailsPanel.LeaveCourseGroupAsync();

        ProfilePanel.Visibility = Visibility.Collapsed;
        EnrollPanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Collapsed;
        DashboardContent.Visibility = Visibility.Visible;
    }

    private async void OnSessionJoinRequested(object? sender, SessionDto session)
    {
        if (_activeSessionId is int currentId && currentId != session.Id)
        {
            try
            {
                await SessionHubService.LeaveSessionAsync(currentId);
            }
            catch
            {
            }
        }

        _activeSessionId = session.Id;
        SessionPanel.Initialize(session);

        DashboardContent.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Collapsed;
        EnrollPanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Visible;

        try
        {
            await SessionHubService.JoinSessionAsync(session.Id);
        }
        catch
        {
        }
    }

    private async void OnLeaveSessionRequested(object? sender, EventArgs e)
    {
        await LeaveSessionAsync();
    }

    private void OnSessionEnded(object? sender, int sessionId)
    {
        Dispatcher.InvokeAsync(async () =>
        {
            if (sessionId != _activeSessionId)
                return;

            await LeaveSessionAsync();
            ShowSessionEndedToast();
        });
    }

    private void OnTranscriptAppended(object? sender, TranscriptionSegment segment)
    {
        Dispatcher.InvokeAsync(() =>
        {
            if (SessionPanel.Visibility == Visibility.Visible)
                SessionPanel.AddSegment(segment);
        });
    }

    private void ShowSessionEndedToast()
    {
        SessionEndedToast.Visibility = Visibility.Visible;
        SessionEndedToast.Opacity = 0;

        var transform = (TranslateTransform)SessionEndedToast.RenderTransform;
        transform.Y = -24;

        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        SessionEndedToast.BeginAnimation(OpacityProperty, fadeIn);

        var slideIn = new DoubleAnimation(-24, 0, TimeSpan.FromMilliseconds(250))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        transform.BeginAnimation(TranslateTransform.YProperty, slideIn);

        _toastTimer?.Stop();
        _toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _toastTimer.Tick += (_, _) =>
        {
            _toastTimer?.Stop();
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
            fadeOut.Completed += (_, _) => SessionEndedToast.Visibility = Visibility.Collapsed;
            SessionEndedToast.BeginAnimation(OpacityProperty, fadeOut);
        };
        _toastTimer.Start();
    }

    private async Task LeaveSessionAsync()
    {
        var sessionId = _activeSessionId;
        _activeSessionId = null;

        if (sessionId is int id)
        {
            try
            {
                await SessionHubService.LeaveSessionAsync(id);
            }
            catch
            {
            }
        }

        SessionPanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Visible;
        CourseDetailsPanel.RefreshSessions();
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
            UserNameTextBlock.Text = currentUser.Username;
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
            (IAuthService)Application.Current.Resources["AuthService"]!,
            (IValidationService)Application.Current.Resources["ValidationService"]!
        );
        signInView.Show();
        Close();
    }

    private async void StudentView_Closed(object? sender, EventArgs e)
    {
        try
        {
            var hubService = Application.Current.Resources["SessionHubService"] as ISessionHubService;
            if (hubService is not null)
                await hubService.DisconnectAsync();
        }
        catch
        {
        }
    }
}
