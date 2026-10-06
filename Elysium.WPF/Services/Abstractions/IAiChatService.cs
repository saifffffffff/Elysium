using Elysium.WPF.Models.Sessions;

namespace Elysium.WPF.Services.Abstractions;

/// <summary>
/// Interface for AI chat communication
/// </summary>
public interface IAiChatService
{
    /// <summary>
    /// Ask the AI a question and stream the answer as SSE chunks
    /// </summary>
    IAsyncEnumerable<AiChatChunk> AskAsync(
        AiChatRequest request,
        CancellationToken cancellationToken = default);
}
