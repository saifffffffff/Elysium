using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the student course details view
/// </summary>
public class StudentCourseDetailsPresenter
{
    private readonly ISessionService _sessionService;

    public event EventHandler<List<SessionDto>>? SessionsLoaded;
    public event EventHandler<string>? SessionsLoadFailed;

    private bool _isLoadingSessions;

    public StudentCourseDetailsPresenter(ISessionService sessionService)
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
}