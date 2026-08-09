namespace Elysium.Shared.Models;

public class Student
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public User User { get; set; } = default!;

    public ICollection<Enrollment> CourseMembers { get; set; } = new List<Enrollment>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<StudentSession> StudentSessions { get; set; } = new List<StudentSession>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
