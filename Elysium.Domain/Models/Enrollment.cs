namespace Elysium.Domain.Models;

public class Enrollment
{

    private Enrollment() { }

    private Enrollment(int studentId , int courseId , DateTime enrollmentDate)
    {
        StudentId = studentId;
        CourseId = courseId;
        EnrollmentDate = enrollmentDate;
    }
    
    public int StudentId { get; private set; }
    public int CourseId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }

    public Student Student { get; set; } = default!;
    public Course Course { get; set; } = default!;

    public static Enrollment Create(int studentId, int courseId)
    {
        return new Enrollment(studentId, courseId, DateTime.UtcNow);
    }


}
