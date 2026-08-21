using Elysium.Api.Hubs;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Elysium.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController(ISessionService sessionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await sessionService.CreateAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("{courseId:int}")]
    public async Task<IActionResult> GetAllByCourseId(int courseId, CancellationToken cancellationToken)
    {

        var result = await sessionService.GetAllByCourseIdAsync(courseId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("{sessionId:int}/end")]
    public async Task<IActionResult> End(int sessionId, CancellationToken cancellationToken)
    {
        var result = await sessionService.EndAsync(sessionId, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Errors);
    }
}