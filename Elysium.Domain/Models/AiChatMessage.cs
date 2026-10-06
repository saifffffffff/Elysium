using Elysium.Domain.Primitives;

namespace Elysium.Domain.Models;

public class AiChatMessage : BaseEntity
{
    public int StudentSessionId { get; private set; }
    public string Question { get; private set; } = default!;
    public string? Answer { get; private set; }
    public DateTime AskedAt { get; private set; }
    public DateTime? AnsweredAt { get; private set; }

    public StudentSession StudentSession { get; set; } = default!;

    public AiChatMessage() { }

    private AiChatMessage(int studentSessionId, string question )
    {
        StudentSessionId = studentSessionId;
        Question = question;
        AskedAt = DateTime.UtcNow;
    }

    public static Result<AiChatMessage> Create(int studentSessionId, string question)
    {
        var result = ValidateStudentSessionId(studentSessionId);
        result.AddResult(ValidateQuestion(question));

        if (!result.IsSuccess)
            return Result<AiChatMessage>.Failure(result);

        return new AiChatMessage(studentSessionId, question);
    }

    public Result SetAnswer(string answer)
    {
        var result = ValidateAnswer(answer);

        if (!result.IsSuccess)
            return result;

        Answer = answer;
        AnsweredAt = DateTime.UtcNow;

        return Result.Success();
    }

    public bool IsAnswered => AnsweredAt is not null;

    private static Result ValidateStudentSessionId(int studentSessionId)
    {
        if (studentSessionId <= 0)
            return Result.Failure("student session id is required");

        return Result.Success();
    }

    private static Result ValidateQuestion(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return Result.Failure("question is required");

        return Result.Success();
    }

    private static Result ValidateAnswer(string answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return Result.Failure("answer is required");

        return Result.Success();
    }
}
