using Elysium.Api.Hubs;
using Elysium.Application.Features.AiChat.DTOs;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Transcription.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Elysium.Application.Features.AiChat.Services;
using System.Text.Json;
namespace Elysium.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController(ISessionService sessionService) : ControllerBase
{

    [HttpGet("{courseId:int}")]
    public async Task<IActionResult> GetAllByCourseId(int courseId, CancellationToken cancellationToken)
    {

        var result = await sessionService.GetAllByCourseIdAsync(courseId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    


}