using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the student courses view
/// </summary>
public class StudentPresenter
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly AuthResponse _currentUser;

    public event EventHandler<IReadOnlyList<CourseDto>>? CoursesLoaded;
    public event EventHandler<string>? CoursesLoadFailed;
    public event EventHandler? EnrollmentSucceeded;
    public event EventHandler<string>? EnrollmentFailed;

    private bool _isLoadingCourses;
    private bool _isEnrolling;

    public StudentPresenter(ICourseService courseService, IEnrollmentService enrollmentService, AuthResponse currentUser)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Load the courses the current student is enrolled in
    /// </summary>
    public async Task HandleLoadCoursesAsync()
    {
        if (_isLoadingCourses)
            return;

        if (_currentUser.StudentId is not int studentId)
        {
            CoursesLoadFailed?.Invoke(this, "Student profile not found. Please contact support.");
            return;
        }

        _isLoadingCourses = true;
        try
        {
            var courses = await _courseService.GetCoursesByStudentAsync(studentId);

            if (courses is null)
            {
                CoursesLoadFailed?.Invoke(this, _courseService.GetLastError() ?? "Failed to load courses.");
                return;
            }

            CoursesLoaded?.Invoke(this, courses);
        }
        finally
        {
            _isLoadingCourses = false;
        }
    }

    /// <summary>
    /// Enroll the current student into a course by code
    /// </summary>
    public async Task HandleEnrollAsync(string courseCode)
    {
        if (_isEnrolling)
            return;

        if (string.IsNullOrWhiteSpace(courseCode))
        {
            EnrollmentFailed?.Invoke(this, "Please enter a course code.");
            return;
        }

        if (_currentUser.StudentId is not int studentId)
        {
            EnrollmentFailed?.Invoke(this, "Student profile not found. Please contact support.");
            return;
        }

        _isEnrolling = true;
        try
        {
            var ok = await _enrollmentService.EnrollStudentAsync(
                new EnrollStudentRequest(studentId, courseCode.Trim()));

            if (ok)
                EnrollmentSucceeded?.Invoke(this, EventArgs.Empty);
            else
                EnrollmentFailed?.Invoke(this, _enrollmentService.GetLastError() ?? "Failed to enroll in course.");
        }
        finally
        {
            _isEnrolling = false;
        }
    }
}