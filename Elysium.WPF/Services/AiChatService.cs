using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Services;

/// <summary>
/// AI chat service for API communication
/// </summary>
public class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "http://localhost:5129/api/";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AiChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiBaseUrl);
    }

    /// <summary>
    /// Ask the AI a question and yield each streamed chunk
    /// </summary>
    public async IAsyncEnumerable<AiChatChunk> AskAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "ai-chats")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request, JsonOptions),
                Encoding.UTF8,
                "application/json")
        };

        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await ApiErrorParser.ExtractErrorMessageAsync(
                response,
                "Failed to get an AI response.");

            throw new InvalidOperationException(message);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: "))
                continue;

            var payload = line["data: ".Length..];

            if (payload == "[DONE]")
                break;

            var chunk = JsonSerializer.Deserialize<AiChatChunk>(payload, JsonOptions);

            if (chunk is not null)
                yield return chunk;
        }
    }
}
