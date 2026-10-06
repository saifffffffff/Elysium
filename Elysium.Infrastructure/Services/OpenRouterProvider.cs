using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Elysium.Application.Features.AiChat.DTOs;
using Elysium.Application.Features.Transcription.Interfaces;
using Elysium.Domain.Models;
using Elysium.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Elysium.Infrastructure.Services;





public class OpenRouterProvider : ILlmProvider
{
    readonly HttpClient _http;
    readonly LlmOptions _options;

    public OpenRouterProvider(IOptions<LlmOptions> options )
    {
        _options = options.Value;
        _http = new HttpClient(); // TODO : check IHttpClient factory
    }

    public async IAsyncEnumerable<string> GenerateResponseAsync(string prompt , IEnumerable<TranscriptSegment> transcriptSegments , IEnumerable<AiChatMessage>? chatContext , CancellationToken cancellationToken = default)
    {
        
        List<object> messages = new();

        if (chatContext is not null)
            messages.AddRange(chatContext.Select(aiChatMessage => new { role = "assistant", content = aiChatMessage.Answer }));
        

        messages.Add(new { role = "system", content = $"you are a student assistant - answer only depending on the transcript : {string.Join(' ', transcriptSegments)}"}); // TODO : Add it in configurations} });
        messages.Add(new { role = "user", content = prompt });


        var content = new
        {
            model = _options.OpenRouter.Model,
            messages = messages,
            stream = _options.Defaults.Stream,
            temperature = _options.Defaults.Temperature,

            //reasoning = new {
            //    enabled = true
            //}
   
        };


        //Console.WriteLine(JsonSerializer.Serialize(content , new JsonSerializerOptions { WriteIndented= true}));
        HttpResponseMessage response;
        
        using (HttpRequestMessage request = new HttpRequestMessage())
        {
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(_options.OpenRouter.BaseUrl);
            request.Headers.Add("Authorization", $"Bearer {_options.OpenRouter.ApiKey}");
            request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            
            response = await _http.SendAsync(request , HttpCompletionOption.ResponseHeadersRead);
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var reader = new StreamReader(stream);
        
        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(line) ) continue;
            if (!line.StartsWith("data: ")) continue;

            string payload = line.Substring("data: ".Length); 
            if (payload == "[DONE]") break;

            //Console.WriteLine(payload);
            //Console.WriteLine(payload);
            //Console.WriteLine();

            if (JsonDocument.Parse(payload).RootElement.TryGetProperty("choices", out var choices))
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("delta", out var delta) && delta.TryGetProperty("content", out var aiAnswerChunk))
                {
                    
                    if (string.IsNullOrEmpty(aiAnswerChunk.GetString()))
                        continue;

                    yield return aiAnswerChunk.GetString() ?? string.Empty;
                }
            }

        }

        
        
    }
}
