using Azure.Core;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Transcription.DTOs;
using Elysium.Infrastructure.Presistence.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;

namespace Elysium.Api.Hubs;

public class CourseHub : Hub
{


    public async Task JoinCourseGroup( int courseId )
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"course:{courseId}");
    }


    public async Task LeaveCourseGroup(int courseId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course:{courseId}");
    }
}

