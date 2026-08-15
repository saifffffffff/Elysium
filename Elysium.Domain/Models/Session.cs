using Elysium.Domain.Primitives;
using System.Runtime.InteropServices;

namespace Elysium.Domain.Models;

public class Session
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    public SessionStatus Status { get; set; } 
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }    

    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;
    
    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<StudentSession> StudentSessions { get; set; } = new List<StudentSession>();
    public ICollection<TranscriptSegment> TranscriptSegments { get; set; } = new List<TranscriptSegment>();


    Session() { }

    private Session(string name, string? description, int courseId)
    {
        Name = name;
        Description = description;
        CourseId = courseId;
    }

    public static Result<Session> Create(string name , string? description , int courseId)
    {
        var result = ValidateName(name);
        result.AddResult(ValidateDescription(ref description));
        result.AddResult(ValidateCourseId(courseId));

        if (!result.IsSuccess)
            return Result<Session>.Failure(result);

        return new Session(name, description, courseId);
    }


    public void Start()
    {
        StartedAt = DateTime.UtcNow;
        Status = SessionStatus.Live;
    }

    public void Finish()
    {
        FinishedAt = DateTime.UtcNow;
        Status = SessionStatus.Finished;
    }

    static Result ValidateName(string name )
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("name is required");

        if (name.Length > 128)
            return Result.Failure("maximum length is 128 characters");

        return Result.Success();
    }
    static Result ValidateDescription(ref string? description)
    {
        if (description is not null && description.Length > 512)
            return Result.Failure("Description max length is 512 characters");

        if (description == string.Empty)
            description = null;

        return Result.Success();
            
    }
    static Result ValidateCourseId ( int courseId)
    {
        if (courseId <= 0)
            return Result.Failure("invalid course id");

        return Result.Success();
    }
    static Result ValidateStartedAt(DateTime startedAt)
    {
        if (startedAt == default)
            return Result.Failure("started at is required");

        return Result.Success();

    }
    static Result ValidateFinishedAt (DateTime? finishedAt , DateTime startedAt)
    {
        if (finishedAt is not null && finishedAt <= startedAt)
            return Result.Failure("finished at must be after started at");

        return Result.Success();
    }


}
