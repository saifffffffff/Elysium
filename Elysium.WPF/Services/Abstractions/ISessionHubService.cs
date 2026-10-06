using Elysium.WPF.Models.Sessions;

namespace Elysium.WPF.Services.Abstractions;

/// <summary>
/// SignalR client for the session hub
/// </summary>
public interface ISessionHubService
{
    /// <summary>
    /// Raised when the hub pushes a newly created session for a joined course group
    /// </summary>
    event EventHandler<SessionDto>? SessionAdded;

    /// <summary>
    /// Raised when the hub pushes a session-ended event for a joined session or course group
    /// </summary>
    event EventHandler<int>? SessionEnded;

    /// <summary>
    /// Raised when the hub pushes a new transcript segment for a joined session
    /// </summary>
    event EventHandler<TranscriptionSegment>? TranscriptAppended;

    /// <summary>
    /// Join the SignalR group for a course to receive live session updates
    /// </summary>
    Task JoinCourseGroupAsync(int courseId);

    /// <summary>
    /// Leave the SignalR group for a course
    /// </summary>
    Task LeaveCourseGroupAsync(int courseId);

    /// <summary>
    /// Start a new session and return its id
    /// </summary>
    Task<int> StartSessionAsync(StartSessionRequest request);

    /// <summary>
    /// Join a session as a student and return the created student session id with the existing transcript
    /// </summary>
    Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest request);

    /// <summary>
    /// Leave a session as a student
    /// </summary>
    Task LeaveSessionAsync(LeaveSessionRequest request);

    /// <summary>
    /// End a live session
    /// </summary>
    Task EndSessionAsync(int sessionId);

    /// <summary>
    /// Stream audio chunks to the server for transcription; completes when the stream ends or is cancelled
    /// </summary>
    Task StreamVoiceAsync(int sessionId, IAsyncEnumerable<byte[]> audioChunks, CancellationToken cancellationToken);

    /// <summary>
    /// Disconnect from the hub and drop all joined groups
    /// </summary>
    Task DisconnectAsync();
}
