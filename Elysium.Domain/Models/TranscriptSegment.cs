using Elysium.Domain.Primitives;

namespace Elysium.Domain.Models;

public class TranscriptSegment
{
    public int Id { get; private set; }
    public int SessionId { get; private set; }
    public int StartTime { get; private set; }
    public int EndTime { get; private set; }
    public string Text { get; private set; } = default!;

    public Session Session { get; set; } = default!;

    public TranscriptSegment() { }

    private TranscriptSegment(int sessionId, int startTime, int endTime, string text)
    {
        SessionId = sessionId;
        StartTime = startTime;
        EndTime = endTime;
        Text = text;
    }

    public static Result<TranscriptSegment> Create(int sessionId, int startTime, int endTime, string text)
    {
        var result = ValidateSessionId(sessionId);
        result.AddResult(ValidateStartTime(startTime));
        result.AddResult(ValidateEndTime(endTime, startTime));
        result.AddResult(ValidateText(text));

        if (!result.IsSuccess)
            return Result<TranscriptSegment>.Failure(result);

        return new TranscriptSegment(sessionId, startTime, endTime, text);
    }

    private static Result ValidateSessionId(int sessionId)
    {
        if (sessionId <= 0)
            return Result.Failure("session id is required");

        return Result.Success();
    }

    private static Result ValidateStartTime(int startTime)
    {
        if (startTime < 0)
            return Result.Failure("start time must be non-negative");

        return Result.Success();
    }

    private static Result ValidateEndTime(int endTime, int startTime)
    {
        if (endTime < 0)
            return Result.Failure("end time must be non-negative");

        if (endTime <= startTime)
            return Result.Failure("end time must be greater than start time");

        return Result.Success();
    }

    private static Result ValidateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure("text is required");

        return Result.Success();
    }
}