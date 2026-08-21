using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Transcription.DTOs;

namespace Elysium.Application.Features.Sessions.Services;

public interface ISessionNotifier
{
    Task NotifySessionCreatedAsync(  int courseId , SessionDto sessionDto, CancellationToken cancellationToken = default);
    Task NotifyTranscriptAppendedAsync(int courseId, TranscriptionSegmentDto segment, CancellationToken cancellationToken = default);
    Task NotifySessionEndedAsync(int courseId, int sessionId, CancellationToken cancellationToken = default);
}

