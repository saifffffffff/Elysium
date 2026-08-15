namespace Elysium.WPF.Models;

/// <summary>
/// Request model for enrolling a student into a course by code
/// </summary>
public record EnrollStudentRequest(int StudentId, string CourseCode);