using Elysium.WPF.Models.Courses;

namespace Elysium.WPF.Services.Abstractions;

/// <summary>
/// Interface for course service
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Create a new course
    /// </summary>
    Task<CreateCourseResponse?> CreateAsync(CreateCourseRequest request);

    /// <summary>
    /// Get all courses belonging to a teacher
    /// </summary>
    Task<IReadOnlyList<CourseDto>?> GetCoursesByTeacherAsync(int teacherId);

    /// <summary>
    /// Get all courses a student is enrolled in
    /// </summary>
    Task<IReadOnlyList<CourseDto>?> GetCoursesByStudentAsync(int studentId);

    /// <summary>
    /// Get the last error message if operation fails
    /// </summary>
    string? GetLastError();
}