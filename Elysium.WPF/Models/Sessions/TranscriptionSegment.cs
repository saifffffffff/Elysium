namespace Elysium.WPF.Models.Sessions;

public record TranscriptionSegment(
    string Text,
    decimal? StartMs,
    decimal? EndMs);