using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Views;

/// <summary>
/// Interaction logic for StudentCourseDetailsView
/// </summary>
public partial class StudentCourseDetailsView : UserControl
{
    private StudentCourseDetailsPresenter? _presenter;
    private CourseDto? _currentCourse;
    private ISessionHubService? _sessionHubService;
    private int _joinedCourseId = -1;
    private readonly ObservableCollection<SessionDto> _liveSessions = new();
    private readonly ObservableCollection<SessionDto> _finishedSessions = new();

    /// <summary>
    /// Raised when the user requests to go back to the dashboard
    /// </summary>
    public event EventHandler? BackRequested;

    /// <summary>
    /// Raised when the student requests to join a live session
    /// </summary>
    public event EventHandler<SessionDto>? SessionJoinRequested;

    public StudentCourseDetailsView()
    {
        InitializeComponent();
        BackButton.Click += BackButton_Click;
        LiveSessionsList.ItemsSource = _liveSessions;
        FinishedSessionsList.ItemsSource = _finishedSessions;
        SessionHubService.SessionAdded += SessionHubService_SessionAdded;
        SessionHubService.SessionEnded += SessionHubService_SessionEnded;
    }

    private ISessionHubService SessionHubService =>
        _sessionHubService ??= (ISessionHubService)Application.Current.Resources["SessionHubService"]!;

    private async Task JoinCourseGroupAsync(int courseId)
    {
        try
        {
            if (_joinedCourseId != -1 && _joinedCourseId != courseId)
                await SessionHubService.LeaveCourseGroupAsync(_joinedCourseId);

            _joinedCourseId = courseId;
            await SessionHubService.JoinCourseGroupAsync(courseId);
        }
        catch
        {
            // Live updates are best-effort; ignore hub failures
        }
    }

    /// <summary>
    /// Leave the SignalR group for the currently shown course
    /// </summary>
    public async Task LeaveCourseGroupAsync()
    {
        if (_joinedCourseId == -1)
            return;

        try
        {
            var courseId = _joinedCourseId;
            _joinedCourseId = -1;
            await SessionHubService.LeaveCourseGroupAsync(courseId);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Wire the presenter and show the given course; safe to call more than once
    /// </summary>
    public void Initialize(CourseDto course)
    {
        _currentCourse = course;
        _ = JoinCourseGroupAsync(course.Id);

        if (_presenter is not null)
        {
            ShowCourse(_currentCourse);
            LoadSessions();
            return;
        }

        _presenter = new StudentCourseDetailsPresenter(
            (ISessionService)Application.Current.Resources["SessionService"]!
        );
        _presenter.SessionsLoaded += Presenter_SessionsLoaded;
        _presenter.SessionsLoadFailed += Presenter_SessionsLoadFailed;

        ShowCourse(_currentCourse);
        LoadSessions();
    }

    /// <summary>
    /// Load the sessions for the currently shown course
    /// </summary>
    private async void LoadSessions()
    {
        if (_presenter is null || _currentCourse is null)
            return;

        SessionsLoadingPanel.Visibility = Visibility.Visible;
        SessionsErrorText.Text = string.Empty;
        LiveSessionsList.Visibility = Visibility.Collapsed;
        LiveEmptyPanel.Visibility = Visibility.Collapsed;
        FinishedSessionsList.Visibility = Visibility.Collapsed;
        FinishedEmptyPanel.Visibility = Visibility.Collapsed;

        await _presenter.HandleLoadSessionsAsync(_currentCourse.Id);
    }

    private void ShowCourse(CourseDto course)
    {
        CourseNameText.Text = course.Name;
        CourseCodeText.Text = $"Course Code : {course.Code}";

        if (string.IsNullOrWhiteSpace(course.Description))
            CourseDescriptionText.Text = string.Empty;
        else
            CourseDescriptionText.Text = course.Description;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Reload the session lists for the currently shown course
    /// </summary>
    public void RefreshSessions() => LoadSessions();

    private void JoinSessionButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: SessionDto session })
            SessionJoinRequested?.Invoke(this, session);
    }

    private void Presenter_SessionsLoaded(object? sender, List<SessionDto> sessions)
    {
        SessionsLoadingPanel.Visibility = Visibility.Collapsed;
        ApplySessions(sessions);
    }

    private void SessionHubService_SessionAdded(object? sender, SessionDto session)
    {
        Dispatcher.InvokeAsync(() =>
        {
            if (_liveSessions.All(s => s.Id != session.Id))
            {
                _liveSessions.Add(session);
                LiveSessionsList.Visibility = Visibility.Visible;
                LiveEmptyPanel.Visibility = Visibility.Collapsed;
            }
        });
    }

    private void SessionHubService_SessionEnded(object? sender, int sessionId)
    {
        Dispatcher.InvokeAsync(() =>
        {
            var ended = _liveSessions.FirstOrDefault(s => s.Id == sessionId);
            if (ended is null)
                return;

            _liveSessions.Remove(ended);
            _finishedSessions.Add(ended with { Status = SessionStatus.Finished });

            if (_liveSessions.Count == 0)
            {
                LiveSessionsList.Visibility = Visibility.Collapsed;
                LiveEmptyPanel.Visibility = Visibility.Visible;
            }

            FinishedSessionsList.Visibility = Visibility.Visible;
            FinishedEmptyPanel.Visibility = Visibility.Collapsed;
        });
    }

    private void ApplySessions(List<SessionDto> sessions)
    {
        var liveSessions = sessions.Where(s => s.Status == SessionStatus.Live).ToList();
        var finishedSessions = sessions.Where(s => s.Status == SessionStatus.Finished).ToList();

        _liveSessions.Clear();
        foreach (var session in liveSessions)
            _liveSessions.Add(session);

        _finishedSessions.Clear();
        foreach (var session in finishedSessions)
            _finishedSessions.Add(session);

        LiveSessionsList.Visibility = liveSessions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        LiveEmptyPanel.Visibility = liveSessions.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        FinishedSessionsList.Visibility = _finishedSessions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        FinishedEmptyPanel.Visibility = _finishedSessions.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Presenter_SessionsLoadFailed(object? sender, string message)
    {
        SessionsLoadingPanel.Visibility = Visibility.Collapsed;
        SessionsErrorText.Text = message;
    }
}