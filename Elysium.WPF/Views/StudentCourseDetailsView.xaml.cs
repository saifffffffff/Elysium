using System.Windows;
using System.Windows.Controls;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;

namespace Elysium.WPF.Views;

/// <summary>
/// Interaction logic for StudentCourseDetailsView
/// </summary>
public partial class StudentCourseDetailsView : UserControl
{
    private CourseDetailsPresenter? _presenter;
    private CourseDto? _currentCourse;

    /// <summary>
    /// Raised when the user requests to go back to the dashboard
    /// </summary>
    public event EventHandler? BackRequested;

    public StudentCourseDetailsView()
    {
        InitializeComponent();
        BackButton.Click += BackButton_Click;
    }

    /// <summary>
    /// Wire the presenter and show the given course; safe to call more than once
    /// </summary>
    public void Initialize(CourseDto course)
    {
        _currentCourse = course;

        if (_presenter is not null)
        {
            ShowCourse(_currentCourse);
            LoadSessions();
            return;
        }

        _presenter = new CourseDetailsPresenter(
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

    private void Presenter_SessionsLoaded(object? sender, List<SessionDto> sessions)
    {
        SessionsLoadingPanel.Visibility = Visibility.Collapsed;

        var liveSessions = sessions.Where(s => s.Status == SessionStatus.Live).ToList();
        var finishedSessions = sessions.Where(s => s.Status == SessionStatus.Finished).ToList();

        if (liveSessions.Count == 0)
        {
            LiveSessionsList.Visibility = Visibility.Collapsed;
            LiveEmptyPanel.Visibility = Visibility.Visible;
        }
        else
        {
            LiveSessionsList.ItemsSource = liveSessions;
            LiveSessionsList.Visibility = Visibility.Visible;
            LiveEmptyPanel.Visibility = Visibility.Collapsed;
        }

        if (finishedSessions.Count == 0)
        {
            FinishedSessionsList.Visibility = Visibility.Collapsed;
            FinishedEmptyPanel.Visibility = Visibility.Visible;
        }
        else
        {
            FinishedSessionsList.ItemsSource = finishedSessions;
            FinishedSessionsList.Visibility = Visibility.Visible;
            FinishedEmptyPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void Presenter_SessionsLoadFailed(object? sender, string message)
    {
        SessionsLoadingPanel.Visibility = Visibility.Collapsed;
        SessionsErrorText.Text = message;
    }
}