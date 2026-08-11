namespace Elysium.Domain.Models;

public class Teacher
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public User User { get; set; } = default!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
