using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Views;

public partial class TeacherView : Window
{
    private enum LeaveAction
    {
        None,
        SignOut,
        Close
    }

    private TeacherCoursesPresenter? _coursesPresenter;
    private LeaveAction _pendingLeaveAction;
    private bool _isEndingSessionForLeave;

    public TeacherView()
    {
        InitializeComponent();
        LoadUserInfo();
        LogoutButton.Click += LogoutButton_Click;
        Closed += TeacherView_Closed;
        Closing += TeacherView_Closing;        ProfilePanel.BackRequested += (_, _) => ShowDashboardContent();
        ProfilePanel.UserDataChanged += (_, _) => LoadUserInfo();
        CoursePanel.BackRequested += (_, _) => ShowDashboardContent();
        CourseDetailsPanel.BackRequested += (_, _) => ShowDashboardContent();
        CourseDetailsPanel.SessionStarted += OnSessionStarted;
        SessionPanel.EndSessionRequested += OnEndSessionRequested;

        ((ISessionHubService)Application.Current.Resources["SessionHubService"]!).TranscriptAppended += OnTranscriptAppended;

        _coursesPresenter = new TeacherCoursesPresenter(
            (ICourseService)Application.Current.Resources["CourseService"]!,
            (IValidationService)Application.Current.Resources["ValidationService"]!,
            (AuthResponse)Application.Current.Resources["CurrentUser"]!
        );
        _coursesPresenter.CoursesLoaded += Presenter_CoursesLoaded;
        _coursesPresenter.CoursesLoadFailed += Presenter_CoursesLoadFailed;
        _coursesPresenter.CourseCreated += Presenter_CourseCreated;

        LoadCourses();
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
        CoursePanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Collapsed;
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

    private async void ShowDashboardContent()
    {
        await CourseDetailsPanel.LeaveCourseGroupAsync();

        ProfilePanel.Visibility = Visibility.Collapsed;
        CoursePanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Collapsed;
        DashboardContent.Visibility = Visibility.Visible;
    }

    private void OnSessionStarted(object? sender, (int SessionId, string Name) e)
    {
        SessionPanel.Initialize(e.SessionId, e.Name);

        DashboardContent.Visibility = Visibility.Collapsed;
        ProfilePanel.Visibility = Visibility.Collapsed;
        CoursePanel.Visibility = Visibility.Collapsed;
        CourseDetailsPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Visible;
    }

    private void OnEndSessionRequested(object? sender, EventArgs e)
    {
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
            CoursePanel.Visibility = Visibility.Collapsed;
            CourseDetailsPanel.Visibility = Visibility.Visible;
        }
    }

    private void OnTranscriptAppended(object? sender, TranscriptionSegment segment)
    {
        Dispatcher.InvokeAsync(() =>
        {
            if (SessionPanel.Visibility == Visibility.Visible)
                SessionPanel.AddSegment(segment);
        });
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
            UserNameTextBlock.Text = currentUser.Username;
            WelcomeTextBlock.Text = $"Hello, {currentUser.FirstName} {currentUser.LastName}!";
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        if (SessionPanel.Visibility == Visibility.Visible)
        {
            ShowLeaveConfirmation(LeaveAction.SignOut);
            return;
        }

        PerformSignOut();
    }

    private void TeacherView_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (SessionPanel.Visibility == Visibility.Visible)
        {
            e.Cancel = true;
            ShowLeaveConfirmation(LeaveAction.Close);
        }
    }

    private void ShowLeaveConfirmation(LeaveAction action)
    {
        _pendingLeaveAction = action;
        LeaveConfirmMessage.Text = action == LeaveAction.SignOut
            ? "You're about to sign out. The session will end and all students will be notified."
            : "You're about to close the window. The session will end and all students will be notified.";

        LeaveConfirmOverlay.Visibility = Visibility.Visible;
        LeaveConfirmOverlay.Opacity = 0;
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        LeaveConfirmOverlay.BeginAnimation(OpacityProperty, fadeIn);
    }

    private void HideLeaveConfirmation()
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
        fadeOut.Completed += (_, _) => LeaveConfirmOverlay.Visibility = Visibility.Collapsed;
        LeaveConfirmOverlay.BeginAnimation(OpacityProperty, fadeOut);
    }

    private void CancelLeaveButton_Click(object sender, RoutedEventArgs e)
    {
        _pendingLeaveAction = LeaveAction.None;
        HideLeaveConfirmation();
    }

    private async void ConfirmLeaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isEndingSessionForLeave || _pendingLeaveAction == LeaveAction.None)
            return;

        _isEndingSessionForLeave = true;
        ConfirmLeaveButton.IsEnabled = false;
        CancelLeaveButton.IsEnabled = false;
        HideLeaveConfirmation();

        try
        {
            var ended = await SessionPanel.TryEndSessionAsync();

            if (!ended)
            {
                _pendingLeaveAction = LeaveAction.None;
                return;
            }

            var action = _pendingLeaveAction;
            _pendingLeaveAction = LeaveAction.None;

            if (action == LeaveAction.SignOut)
                PerformSignOut();
            else
                Close();
        }
        finally
        {
            _isEndingSessionForLeave = false;
            ConfirmLeaveButton.IsEnabled = true;
            CancelLeaveButton.IsEnabled = true;
        }
    }

    private void PerformSignOut()
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

    private async void TeacherView_Closed(object? sender, EventArgs e)
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
