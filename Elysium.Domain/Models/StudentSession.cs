using Elysium.Domain.Primitives;

namespace Elysium.Domain.Models;

public class StudentSession : BaseEntity
{
    public int StudentId { get; private set; }
    public int SessionId { get; private set; }
    public bool IsInSession { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Student Student { get; set; } = default!;
    public Session Session { get; set; } = default!;
    public ICollection<ConfusionFlag> ConfusionFlags { get; set; } = new List<ConfusionFlag>();
    public ICollection<AiChatMessage> AiChatMessages { get; set; } = new List<AiChatMessage>();

    private StudentSession() { }                     

    private StudentSession(int studentId, int sessionId)
    {
        StudentId = studentId;
        SessionId = sessionId;
        IsInSession = true; // an attendance row is born "in session"
        JoinedAt = DateTime.UtcNow;
    }

    public static Result<StudentSession> Create(int studentId, int sessionId)
    {
        var result = ValidateStudentId(studentId);
        result.AddResult(ValidateSessionId(sessionId));

        if (!result.IsSuccess)
            return Result<StudentSession>.Failure(result);

        return Result<StudentSession>.Success(new StudentSession(studentId, sessionId));
    }

    private static Result ValidateStudentId(int studentId)
    {
        if (studentId <= 0)
            return Result.Failure("student id is required");

        return Result.Success();
    }

    private static Result ValidateSessionId(int sessionId)
    {
        if (sessionId <= 0)
            return Result.Failure("session id is required");

        return Result.Success();
    }

    public bool HasLeft => !IsInSession;

    public void Leave()
    {
        if (!IsInSession)
            return;

        IsInSession = false;
    }

    public void Rejoin()
    {
        if (IsInSession)
            return;

        IsInSession = true;
    }
}