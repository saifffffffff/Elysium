using Elysium.Application.Features.Transcription.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Infrastructure.Services;

public class QwenLlmService : ILlmService
{
    public Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return null;
    }
}
