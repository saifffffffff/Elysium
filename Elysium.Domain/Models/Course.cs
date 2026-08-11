namespace Elysium.Domain.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Code { get; set; } = default!;
    public int TeacherId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Teacher Teacher { get; set; } = default!;
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Student> Members { get; set; } = new List<Student>();
}
