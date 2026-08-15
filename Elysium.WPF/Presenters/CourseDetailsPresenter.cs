using Elysium.WPF.Models.Courses;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Services;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the course details view
/// </summary>
public class CourseDetailsPresenter
{
    private readonly ISessionService _sessionService;

    public event EventHandler<List<SessionDto>>? SessionsLoaded;
    public event EventHandler<string>? SessionsLoadFailed;
    public event EventHandler<string>? SessionStartRequested;

    private bool _isLoadingSessions;

    public CourseDetailsPresenter(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Load the sessions belonging to a course
    /// </summary>
    public async Task HandleLoadSessionsAsync(int courseId)
    {
        if (_isLoadingSessions)
            return;

        _isLoadingSessions = true;
        try
        {
            var sessions = await _sessionService.GetSessionsByCourseIdAsync(courseId);

            if (sessions is null)
            {
                SessionsLoadFailed?.Invoke(this, _sessionService.GetLastError() ?? "Failed to load sessions.");
                return;
            }

            SessionsLoaded?.Invoke(this, sessions);
        }
        finally
        {
            _isLoadingSessions = false;
        }
    }

    /// <summary>
    /// Raise a request to start a session for the given course
    /// </summary>
    public void HandleStartSessionRequested(CourseDto course)
    {
        SessionStartRequested?.Invoke(this, $"[Dummy Action] Session creation UI ready for course: {course.Name}");
    }
}