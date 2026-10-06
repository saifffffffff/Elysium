using System.Text.Json;
using Elysium.Application.Features.AiChat.DTOs;
using Elysium.Application.Features.AiChat.Services;
using Elysium.Application.Features.Transcription.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Api.Controllers;

[ApiController]
[Route("api/ai-chats")]
public class AiChatController(IAiChatService aiChatService ) :  ControllerBase
{

    [HttpPost]
    public async Task AskQuestion(  [FromBody] StreamChatRequest request )
    {

        Console.WriteLine("request arrived");
        HttpContext.Response.ContentType = "text/event-stream";

        await foreach (var chunk in aiChatService.StreamChat(request))
        {
            await HttpContext.Response.WriteAsync("data: ");
            await JsonSerializer.SerializeAsync(HttpContext.Response.Body, chunk);
            await HttpContext.Response.WriteAsync("\n\n");
        }
    }


}
