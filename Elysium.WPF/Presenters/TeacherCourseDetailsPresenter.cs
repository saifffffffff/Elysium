using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the teacher course details view
/// </summary>
public class TeacherCourseDetailsPresenter
{
    private readonly ISessionService _sessionService;
    private readonly IValidationService _validationService;

    public event EventHandler<List<SessionDto>>? SessionsLoaded;
    public event EventHandler<string>? SessionsLoadFailed;
    public event EventHandler<int>? SessionCreated;
    public event EventHandler<string>? SessionCreateFailed;
    public event EventHandler<List<ValidationError>>? ValidationErrorsChanged;

    private bool _isLoadingSessions;
    private bool _isCreatingSession;

    public TeacherCourseDetailsPresenter(ISessionService sessionService, IValidationService validationService)
    {
        _sessionService = sessionService;
        _validationService = validationService;
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
    /// Validate and create a new session for the given course
    /// </summary>
    public async Task HandleCreateSessionAsync(CourseDto course, string name, string? description)
    {
        if (_isCreatingSession)
            return;

        var errors = _validationService.ValidateCreateSession(name, description);

        if (errors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, errors);
            return;
        }

        var request = new CreateSessionRequest(
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            course.Id);

        _isCreatingSession = true;
        try
        {
            var sessionId = await _sessionService.CreateAsync(request);

            if (sessionId is null)
            {
                SessionCreateFailed?.Invoke(this, _sessionService.GetLastError() ?? "Failed to create session.");
                return;
            }

            ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
            SessionCreated?.Invoke(this, sessionId.Value);
        }
        finally
        {
            _isCreatingSession = false;
        }
    }
}
