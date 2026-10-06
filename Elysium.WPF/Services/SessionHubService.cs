using Elysium.WPF.Models.Sessions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Services;

/// <summary>
/// SignalR client for the session hub
/// </summary>
public class SessionHubService : ISessionHubService
{
    private const string CourseHubUrl = "http://localhost:5129/hub/course";
    private const string SessionHubUrl = "http://localhost:5129/hub/session";

    private readonly HubConnection _courseConnection;
    private readonly HubConnection _sessionConnection;
    private readonly HashSet<int> _joinedCourseIds = new();
    private readonly object _lock = new();

    public event EventHandler<SessionDto>? SessionAdded;
    public event EventHandler<int>? SessionEnded;
    public event EventHandler<TranscriptionSegment>? TranscriptAppended;

    public SessionHubService()
    {
        _courseConnection = new HubConnectionBuilder()
            .WithUrl(CourseHubUrl)
            .WithAutomaticReconnect()
            .AddJsonProtocol(options => options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true)
            .Build();

        _sessionConnection = new HubConnectionBuilder()
            .WithUrl(SessionHubUrl)
            .WithAutomaticReconnect()
            .AddJsonProtocol(options => options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true)
            .Build();

        _courseConnection.On<SessionDto>("SessionAdded", session =>
        {
            SessionAdded?.Invoke(this, session);
        });

        _courseConnection.On<int>("SessionEnded", sessionId =>
        {
            SessionEnded?.Invoke(this, sessionId);
        });

        _sessionConnection.On<TranscriptionSegment>("TranscriptAppended", segment =>
        {
            TranscriptAppended?.Invoke(this, segment);
        });

        _courseConnection.Reconnected += async _ =>
        {
            await RejoinCourseGroupsAsync();
        };
    }

    /// <summary>
    /// Join the SignalR group for a course to receive live session updates
    /// </summary>
    public async Task JoinCourseGroupAsync(int courseId)
    {
        lock (_lock)
        {
            if (!_joinedCourseIds.Add(courseId))
                return;
        }

        await EnsureConnectedAsync(_courseConnection);
        await _courseConnection.InvokeAsync("JoinCourseGroup", courseId);
    }

    /// <summary>
    /// Leave the SignalR group for a course
    /// </summary>
    public async Task LeaveCourseGroupAsync(int courseId)
    {
        lock (_lock)
        {
            _joinedCourseIds.Remove(courseId);
        }

        if (_courseConnection.State == HubConnectionState.Connected)
            await _courseConnection.InvokeAsync("LeaveCourseGroup", courseId);
    }

    /// <summary>
    /// Start a new session and return its id
    /// </summary>
    public async Task<int> StartSessionAsync(StartSessionRequest request)
    {
        await EnsureConnectedAsync(_sessionConnection);
        return await _sessionConnection.InvokeAsync<int>("StartSession", request);
    }

    /// <summary>
    /// Join a session as a student and return the created student session id with the existing transcript
    /// </summary>
    public async Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest request)
    {
        await EnsureConnectedAsync(_sessionConnection);
        return await _sessionConnection.InvokeAsync<JoinSessionResponse>("JoinSession", request);
    }

    /// <summary>
    /// Leave a session as a student
    /// </summary>
    public async Task LeaveSessionAsync(LeaveSessionRequest request)
    {
        if (_sessionConnection.State == HubConnectionState.Connected)
            await _sessionConnection.InvokeAsync("LeaveSession", request);
    }

    /// <summary>
    /// End a live session
    /// </summary>
    public async Task EndSessionAsync(int sessionId)
    {
        await EnsureConnectedAsync(_sessionConnection);
        await _sessionConnection.InvokeAsync("EndSession", sessionId);
    }

    /// <summary>
    /// Stream audio chunks to the server for transcription; completes when the stream ends or is cancelled
    /// </summary>
    public async Task StreamVoiceAsync(int sessionId, IAsyncEnumerable<byte[]> audioChunks, CancellationToken cancellationToken)
    {
        await EnsureConnectedAsync(_sessionConnection);
        await _sessionConnection.InvokeAsync("HandleVoiceData", sessionId, audioChunks, cancellationToken);
    }

    /// <summary>
    /// Disconnect from the hubs and drop all joined groups
    /// </summary>
    public async Task DisconnectAsync()
    {
        lock (_lock)
        {
            _joinedCourseIds.Clear();
        }

        if (_courseConnection.State != HubConnectionState.Disconnected)
            await _courseConnection.StopAsync();

        if (_sessionConnection.State != HubConnectionState.Disconnected)
            await _sessionConnection.StopAsync();
    }

    private static async Task EnsureConnectedAsync(HubConnection connection)
    {
        if (connection.State == HubConnectionState.Disconnected)
            await connection.StartAsync();
    }

    private async Task RejoinCourseGroupsAsync()
    {
        List<int> courseIds;
        lock (_lock)
        {
            courseIds = _joinedCourseIds.ToList();
        }

        if (courseIds.Count == 0)
            return;

        await EnsureConnectedAsync(_courseConnection);
        foreach (var courseId in courseIds)
        {
            await _courseConnection.InvokeAsync("JoinCourseGroup", courseId);
        }
    }
}
