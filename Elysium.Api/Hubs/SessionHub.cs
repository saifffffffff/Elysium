using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Azure.Core;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Transcription.DTOs;
using Elysium.Application.Features.Transcription.Services;
using Elysium.Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace Elysium.Api.Hubs;


public class ConnectionTracker
{
    private readonly Dictionary<string, HashSet<string>> _groups = new();
    private readonly Dictionary<string, string> _connections = new();
    public IEnumerable<string>? GetAllClients(string groupName)
    {
        if (_groups.TryGetValue(groupName, out HashSet<string>? clients))
        {
            return clients;
        }

        return null;
    }

    public Dictionary<string,HashSet<string>>.KeyCollection Groups => _groups.Keys; 
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
public class SessionHub(ISpeechToTextService sttService , ISessionService sessionService , ConnectionTracker connectionTracker ) : Hub
{

    public async Task<int> StartSession(StartSessionRequest startSessionRequest)
    {
        var sessionCreationResult = await sessionService.StartAsync(startSessionRequest);
        
        if (!sessionCreationResult.IsSuccess)
            throw new HubException(string.Join( ' ' , sessionCreationResult.Errors));

        int sessionId = sessionCreationResult.Value;

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session:{sessionId}");
        }   
        catch (HubException ex)
        {
            await sessionService.DeleteByIdAsync(sessionId);
            throw new HubException("failed to start session");
        }

        connectionTracker.Add($"session:{sessionId}", Context.ConnectionId);
        
        Console.WriteLine("Joined");
        return sessionId;
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

    public async Task<JoinSessionResponse> JoinSession(JoinSessionRequest request)
    {

        var result = await sessionService.JoinAsync(request);
        
        if (!result.IsSuccess)
            throw new HubException(string.Join(' ', result.Errors));

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session:{request.sessionId}");
        }
        catch (HubException ex)
        {
            await sessionService.LeaveAsync(new LeaveSessionRequest(request.studentId , request.sessionId));
            throw ex;
        }

        connectionTracker.Add($"session:{request.sessionId}", Context.ConnectionId);
        Console.WriteLine("Joined");

        return result.Value!;


    }
    public async Task LeaveSession(LeaveSessionRequest request)
    {
        var result = await sessionService.LeaveAsync(request);
        
        if (!result.IsSuccess)
            throw new HubException(string.Join(';', result.Errors));


        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session:{request.sessionId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to remove from group: {ex.Message}");
        }
        finally
        {
            connectionTracker.Remove($"session:{request.sessionId}", Context.ConnectionId);
            Console.WriteLine("left");
        }
    }


    public async Task HandleVoiceData(int sessionId, IAsyncEnumerable<byte[]> audioChunks)
    {
        Console.WriteLine($"Voice recieved");
        await sttService.TranscribeSessionAsync(sessionId, audioChunks.Select(b => new ReadOnlyMemory<byte>(b)), Context.ConnectionAborted);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        
        //connectionTracker.Groups
        //int sessionId = default;
        //var groups = connectionTracker.Groups;
        //foreach ( var group in groups)
        //{
        //    var groupName = connectionTracker.GetAllClients(group)!.FirstOrDefault(connectionId => connectionId == Context.ConnectionId);
        //    if ( groupName is not null)
        //    {
        //        sessionId = int.Parse(groupName.Split(':')[1]);
        //        break;
        //    }
        //}

        //if (sessionId == 0)
        //    return;
        
        //await LeaveSession(sessionId);
        
        await base.OnDisconnectedAsync(exception);
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