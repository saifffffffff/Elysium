using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Transcription.Interfaces;

public interface ILlmService
{
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);
}
