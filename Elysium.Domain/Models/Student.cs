using Elysium.Domain.Primitives;

namespace Elysium.Domain.Models;

public class Student
{
    public int Id { get; private set; }
    public int UserId { get; private set; }

    public User User { get; private set; } = default!;

    public Student() { }
    
    private Student(int userId)
    {
        UserId = userId;
    }

    public static Result<Student> Create(int userId)
    {

        var result = ValidateUserId(userId);

        if (!result.IsSuccess)
            return Result<Student>.Failure(result);

        return Result<Student>.Success(new Student(userId));


    }

    static Result ValidateUserId(int userId )
    {
        if (userId <= 0)
            return Result.Failure("Invalid user id");

        return Result.Success();
    }

    public ICollection<Enrollment> CourseMembers { get; set; } = new List<Enrollment>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<StudentSession> StudentSessions { get; set; } = new List<StudentSession>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
