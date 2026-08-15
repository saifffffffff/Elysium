namespace Elysium.WPF.Models.Courses;

public record CreateCourseRequest(string Name, string? Description, int TeacherId);