using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Transcription.Interfaces;
using Elysium.Application.Features.Transcription.Options;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.Extensions.Options;

namespace Elysium.Application.Features.Transcription.Services;

public sealed class SpeechToTextService(
        ISessionRepository sessionRepository,
        ITranscriptSegmentRepository transcriptSegmentRepository,
        IUnitOfWork unitOfWork,
        ISessionNotifier sessionNotifier,
        ITranscriptionProvider transcriptionProvider,
        IOptions<TranscriptionStreamOptions> streamOptions) : ISpeechToTextService
    {
    
    
    public async Task TranscribeSessionAsync(int sessionId, IAsyncEnumerable<ReadOnlyMemory<byte>> audioChunks, CancellationToken cancelationToken)
    {
        try
        {
            var sessionExists = await sessionRepository.ExistsAsync(session => session.Id == sessionId, cancelationToken);

            if (!sessionExists)
                throw new InvalidOperationException($"Session {sessionId} not found");

            Console.WriteLine($"[STT] Starting transcription for sessionId={sessionId}");

            await foreach (var segment in transcriptionProvider.StreamAsync(audioChunks, streamOptions.Value, cancelationToken))
            {
                if (string.IsNullOrWhiteSpace(segment.Text))
                    continue;

                Console.WriteLine($"[STT] Segment received: {segment.Text}");

                var segmentResult = TranscriptSegment.Create(sessionId, (int)(segment.StartMs ?? 0), (int)(segment.EndMs ?? 0), segment.Text);

                if (!segmentResult.IsSuccess)
                    continue;

                await transcriptSegmentRepository.AddAsync(segmentResult.Value!);

                await unitOfWork.SaveChangesAsync(cancelationToken);

                // 3. Broadcast to the session group.
                await sessionNotifier.NotifyTranscriptAppendedAsync(sessionId, segment, cancelationToken);
            }

            Console.WriteLine($"[STT] Transcription completed for sessionId={sessionId}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"[STT] Transcription cancelled for sessionId={sessionId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] TranscribeSessionAsync failed for sessionId={sessionId}: {ex.Message}");
            Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
            throw;
        }
    }
}

