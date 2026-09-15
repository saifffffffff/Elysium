using Elysium.Application.Features.Transcription.DTOs;

namespace Elysium.Application.Features.Transcription.Interfaces;

public interface ITranscriptionProvider
{
    IAsyncEnumerable<TranscriptionSegmentDto> StreamAsync(IAsyncEnumerable<ReadOnlyMemory<byte>> audioChunks,CancellationToken ct);
}