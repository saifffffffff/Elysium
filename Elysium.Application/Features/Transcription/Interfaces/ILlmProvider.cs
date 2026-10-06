using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Elysium.Application.Features.AiChat.DTOs;
using Elysium.Domain.Models;

namespace Elysium.Application.Features.Transcription.Interfaces;

public interface ILlmProvider
{
    IAsyncEnumerable<string> GenerateResponseAsync(string prompt , IEnumerable<TranscriptSegment> transcriptSegments, IEnumerable<AiChatMessage> chatContext, CancellationToken cancellationToken = default);
}
