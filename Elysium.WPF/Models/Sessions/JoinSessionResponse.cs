namespace Elysium.WPF.Models.Sessions;

public record JoinSessionResponse(
    int SessionStudentId,
    IReadOnlyList<TranscriptionSegment> Transcript);
