namespace Elysium.Application.Features.Transcription.DTOs;

public record TranscriptionSegmentDto( string Text, decimal? StartMs, decimal? EndMs);