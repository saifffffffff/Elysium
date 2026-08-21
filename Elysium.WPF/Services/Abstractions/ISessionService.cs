using Elysium.WPF.Models.Sessions;

namespace Elysium.WPF.Services.Abstractions;

/// <summary>
/// Interface for session service
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Create a new session
    /// </summary>
    Task<int?> CreateAsync(CreateSessionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all sessions belonging to a course
    /// </summary>
    Task<List<SessionDto>?> GetSessionsByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// End a live session
    /// </summary>
    Task<bool> EndSessionAsync(int sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the last error message if operation fails
    /// </summary>
    string? GetLastError();
}