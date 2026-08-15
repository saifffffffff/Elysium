using Elysium.WPF.Models.Sessions;

namespace Elysium.WPF.Services;

/// <summary>
/// Interface for session service
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Get all sessions belonging to a course
    /// </summary>
    Task<List<SessionDto>?> GetSessionsByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the last error message if operation fails
    /// </summary>
    string? GetLastError();
}