using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Transcription.Services;

public interface ISpeechToTextService
{
    Task TranscribeSessionAsync(int sessionId, IAsyncEnumerable<ReadOnlyMemory<byte>> audioChunks,  CancellationToken ct);
}
