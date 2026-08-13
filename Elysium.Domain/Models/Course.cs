using Elysium.Domain.Primitives;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Elysium.Domain.Models;

public class Course
{
    public int Id { get; private  set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string Code { get; private set; } = default!;
    public int TeacherId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Course() { }

    private Course(string Name , string? Description , string Code , int TeacherId  , DateTime CreatedAt)
    {
        this.Name = Name;
        this.Description = Description;
        this.Code = Code;
        this.TeacherId = TeacherId;
        this.CreatedAt = CreatedAt;
    }


    public static Result<Course> Create(string Name , string? Description , string Code , int TeacherId )
    {
        var result = ValidateName(Name);
        result.AddResult(ValidateDescription(ref Description));
        result.AddResult(ValidateCode(Code));

        if (!result.IsSuccess)
            return Result<Course>.Failure(result);

        return new Course(Name, Description, Code, TeacherId, DateTime.UtcNow);
    }

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("name is  required");

        if (name.Length > 128)
            return Result.Failure("name max length is 128 characters");


        return Result.Success();
    }

    private static Result ValidateDescription(ref string? description)
    {

        if (description is not null && description.Length > 512)
            return Result.Failure("Description max length is 512 characters");

        if (description == string.Empty)
            description = null;

        return Result.Success();
            
    }

    private static Result ValidateCode(string code )
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("code is required");

        if (code.Length != 6)
            return Result.Failure("code length must be 6");

        if (!Regex.IsMatch(code, "^[a-zA-Z0-9]+$"))
            return Result.Failure("Code must contain only letters and numbers.");

        return Result.Success();
    }


    public Teacher Teacher { get; set; } = default!;
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Student> Members { get; set; } = new List<Student>();
}
