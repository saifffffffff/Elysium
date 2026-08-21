using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Transcription.DTOs;
using Elysium.Application.Features.Transcription.Services;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace Elysium.Api.Hubs;


public class ConnectionTracker
{
    private readonly Dictionary<string, HashSet<string>> _groups = new();

    public IEnumerable<string>? GetAllClients(string groupName)
    {
        if (_groups.TryGetValue(groupName, out HashSet<string>? clients))
        {
            return clients;
        }

        return null;
    }
    public void Add(string groupName , string connectionId)
    {
        if ( !_groups.ContainsKey(groupName))
        {
            var hashSet = new HashSet<string>();
            hashSet.Add(connectionId);
            _groups.Add(groupName, hashSet);
        }

        else
        {
            _groups[groupName].Add(connectionId);
        }
    }
    
    public void Remove(string groupName , string connectionId)
    {

        _groups[groupName].Remove(connectionId);

        if (_groups[groupName].Count == 0)
            _groups.Remove(groupName);
    }

}
public class SessionHub(ISpeechToTextService sttService , ISessionService sessionService , ConnectionTracker connectionTracker) : Hub
{


public async Task JoinSession(int sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session:{sessionId}");
        Console.WriteLine("Joined");
        connectionTracker.Add($"session:{sessionId}", Context.ConnectionId);
    }

    public async Task LeaveSession(int sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session:{sessionId}");
        Console.WriteLine("left");
        
        connectionTracker.Remove($"session:{sessionId}", Context.ConnectionId);
    }

    public async Task EndSession(int sessionId)
    {
        var result = await sessionService.EndAsync(sessionId);
        Console.WriteLine("ended");

        if (!result.IsSuccess)
            throw new HubException("Failed to end the session.");

        var connectionIds = connectionTracker.GetAllClients($"session:{sessionId}");
        
        if (connectionIds is null)
            return;

        foreach ( var connectionId in connectionIds)
        {
            await Groups.RemoveFromGroupAsync(connectionId, $"session:{sessionId}");
        }
    }

    public async Task HandleVoiceData(int sessionId, IAsyncEnumerable<byte[]> audioChunks)
    {
        Console.WriteLine($"Voice recieved");
        await sttService.TranscribeSessionAsync(sessionId, audioChunks.Select(b => new ReadOnlyMemory<byte>(b)), Context.ConnectionAborted);
    }


}

public class SessionNotifier(IHubContext<CourseHub> courseHub, IHubContext<SessionHub> sessionHub) : ISessionNotifier
{
    public async Task NotifySessionCreatedAsync(int courseId, SessionDto sessionDto, CancellationToken cancellationToken = default)
    {
        await courseHub.Clients.Group($"course:{courseId}").SendAsync("SessionAdded", sessionDto, cancellationToken);
    }

    public async Task NotifyTranscriptAppendedAsync(int sessionId, TranscriptionSegmentDto segment, CancellationToken cancellationToken = default)
    {
        await sessionHub.Clients.Group($"session:{sessionId}").SendAsync("TranscriptAppended", segment, cancellationToken);
    }

    public async Task NotifySessionEndedAsync(int courseId, int sessionId, CancellationToken cancellationToken = default)
    {
        await courseHub.Clients.Group($"course:{courseId}").SendAsync("SessionEnded", sessionId, cancellationToken);
    }
}