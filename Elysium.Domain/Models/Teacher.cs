using Elysium.Domain.Primitives;
using System.ComponentModel;

namespace Elysium.Domain.Models;

public class Teacher
{

    public int Id { get; private set; }
    public int UserId { get; private set; }

    public Teacher() { }

    private Teacher(int userId )
    {
        UserId = userId;
    }

    public static Result<Teacher> Create(int userId )
    {
        var result = ValidateUserId(userId);

        if (!result.IsSuccess)
            return Result<Teacher>.Failure(result);

        return Result<Teacher>.Success(new Teacher(userId));

    }
    static Result ValidateUserId(int userId)
    {
        if (userId <= 0)
            return Result.Failure("Invalid user id");

        return Result.Success();
    }

    public User User { get; set; } = default!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
