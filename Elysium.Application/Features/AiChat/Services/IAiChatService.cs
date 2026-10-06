using Elysium.Application.Features.AiChat.DTOs;

namespace Elysium.Application.Features.AiChat.Services;

public interface IAiChatService
{
    IAsyncEnumerable<Chunk> StreamChat(StreamChatRequest request);
}