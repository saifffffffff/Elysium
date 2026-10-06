using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using Elysium.Domain.Primitives;

namespace Elysium.Application.Features.AiChat.DTOs;

public class Chunk
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = default!;

    [JsonPropertyName("isErrorMessage")]
    public bool IsErrorMessage { get; set; } = false;
}

public class ChatStreamChunk
{
    
    [JsonPropertyName("choices")]
    public List<ChunkChoice> Choices { get; set; }

    public bool IsErrorChunk { get; private set; } = false;

    string _error;
    
    public string Error
    {
        set { _error = value; IsErrorChunk = true; }
        get => _error;
    }

    public static ChatStreamChunk ErrorChunk(string errorMessage) {
        var chunk = new ChatStreamChunk();
        chunk.Error = errorMessage;
        return chunk;
    }
}

public class ChunkChoice
{
    [JsonPropertyName("delta")]
    public ChunkDelta Delta { get; set; }
}

public class ChunkDelta
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}
