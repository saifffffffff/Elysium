namespace Elysium.WPF.Models.Courses;

public record CourseDto(int Id, string Name, string? Description, string Code, int TeacherId, DateTime CreatedAt);