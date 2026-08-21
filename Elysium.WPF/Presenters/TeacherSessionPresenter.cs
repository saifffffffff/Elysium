using System.Collections.ObjectModel;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the teacher session view
/// </summary>
public class TeacherSessionPresenter
{
    private readonly ISessionService _sessionService;
    private readonly ISessionHubService _sessionHubService;
    private readonly IMicrophoneService _microphone;
    private CancellationTokenSource? _streamCts;
    private bool _isEndingSession;
    private int? _sessionId;

    public event EventHandler? MuteStateChanged;
    public event EventHandler? SessionEnded;
    public event EventHandler<string>? EndSessionFailed;
    public event EventHandler<string>? MicFailed;

    /// <summary>
    /// The live transcript segments for the current session
    /// </summary>
    public ObservableCollection<TranscriptionSegment> Segments { get; } = new();

    /// <summary>
    /// The name of the current session
    /// </summary>
    public string SessionName { get; private set; } = string.Empty;

    /// <summary>
    /// Whether the microphone is currently muted
    /// </summary>
    public bool IsMuted { get; private set; }

    public TeacherSessionPresenter(ISessionService sessionService, ISessionHubService sessionHubService, IMicrophoneService microphone)
    {
        _sessionService = sessionService;
        _sessionHubService = sessionHubService;
        _microphone = microphone;
        _microphone.Failed += Microphone_Failed;
    }

    /// <summary>
    /// Prepare the presenter for a session; safe to call more than once
    /// </summary>
    public void Initialize(int sessionId, string name)
    {
        _sessionId = sessionId;
        SessionName = name;
        IsMuted = false;
        _microphone.IsMuted = false;
        Segments.Clear();

        _ = StartVoiceStreamAsync(sessionId);
    }

    /// <summary>
    /// Append a finalized transcript segment
    /// </summary>
    public void AddSegment(TranscriptionSegment segment)
    {
        Segments.Add(segment);
    }

    /// <summary>
    /// Toggle the microphone mute state
    /// </summary>
    public void ToggleMute()
    {
        IsMuted = !IsMuted;
        _microphone.IsMuted = IsMuted;
        MuteStateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Stop the microphone, cancel the audio stream, and release the capture device
    /// </summary>
    public void Stop()
    {
        _streamCts?.Cancel();
        _microphone.Stop();
    }

    /// <summary>
    /// End the current session; returns true when the session was ended successfully
    /// </summary>
    public async Task<bool> HandleEndSessionAsync()
    {
        if (_isEndingSession || _sessionId is not int sessionId)
            return false;

        _isEndingSession = true;
        try
        {
            Stop();

            if (await _sessionService.EndSessionAsync(sessionId))
            {
                SessionEnded?.Invoke(this, EventArgs.Empty);
                return true;
            }

            EndSessionFailed?.Invoke(this, _sessionService.GetLastError() ?? "Failed to end session.");
            return false;
        }
        finally
        {
            _isEndingSession = false;
        }
    }

    private async Task StartVoiceStreamAsync(int sessionId)
    {
        _streamCts?.Cancel();
        _streamCts?.Dispose();
        _streamCts = new CancellationTokenSource();
        var cancellationToken = _streamCts.Token;

        try
        {
            await _sessionHubService.JoinSessionAsync(sessionId);
            await _microphone.StartAsync();
            await _sessionHubService.StreamVoiceAsync(sessionId, _microphone.GetChunks(cancellationToken), cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            MicFailed?.Invoke(this, ex.Message);
        }
    }

    private void Microphone_Failed(object? sender, string message)
    {
        _streamCts?.Cancel();
        MicFailed?.Invoke(this, message);
    }
}