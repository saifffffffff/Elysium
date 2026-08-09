namespace Elysium.Shared.Models;

public class Enrollment
{

    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }

    public Student Student { get; set; } = default!;
    public Course Course { get; set; } = default!;
}
