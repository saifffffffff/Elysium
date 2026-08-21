using Elysium.Application.Features.Transcription.DTOs;
using Elysium.Application.Features.Transcription.Options;

namespace Elysium.Application.Features.Transcription.Interfaces;

public interface ITranscriptionProvider
{
    IAsyncEnumerable<TranscriptionSegmentDto> StreamAsync(
        IAsyncEnumerable<ReadOnlyMemory<byte>> audioChunks,
        TranscriptionStreamOptions options,
        CancellationToken ct);
}