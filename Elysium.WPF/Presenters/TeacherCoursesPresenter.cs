using Elysium.WPF.Models;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Services;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the teacher courses view
/// </summary>
public class TeacherCoursesPresenter
{
    private readonly ICourseService _courseService;
    private readonly IValidationService _validationService;
    private readonly AuthResponse _currentUser;

    public event EventHandler<CreateCourseResponse>? CourseCreated;
    public event EventHandler<string>? CourseCreateFailed;
    public event EventHandler<IReadOnlyList<CourseDto>>? CoursesLoaded;
    public event EventHandler<string>? CoursesLoadFailed;
    public event EventHandler<List<ValidationError>>? ValidationErrorsChanged;

    private bool _isLoadingCourses;

    public TeacherCoursesPresenter(ICourseService courseService, IValidationService validationService, AuthResponse currentUser)
    {
        _courseService = courseService;
        _validationService = validationService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Load the courses belonging to the current teacher
    /// </summary>
    public async Task HandleLoadCoursesAsync()
    {
        if (_isLoadingCourses)
            return;

        if (_currentUser.TeacherId is not int teacherId)
        {
            CoursesLoadFailed?.Invoke(this, "Teacher profile not found. Please contact support.");
            return;
        }

        _isLoadingCourses = true;
        try
        {
            var courses = await _courseService.GetCoursesByTeacherAsync(teacherId);

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
    /// Validate and create a new course
    /// </summary>
    public async Task HandleCreateCourseAsync(string name, string? description)
    {
        var errors = _validationService.ValidateCreateCourse(name, description);

        if (errors.Count > 0)
        {
            ValidationErrorsChanged?.Invoke(this, errors);
            return;
        }

        if (_currentUser.TeacherId is not int teacherId)
        {
            CourseCreateFailed?.Invoke(this, "Teacher profile not found. Please contact support.");
            return;
        }

        var request = new CreateCourseRequest(
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            teacherId);

        var result = await _courseService.CreateAsync(request);

        if (result is null)
        {
            CourseCreateFailed?.Invoke(this, _courseService.GetLastError() ?? "Failed to create course.");
            return;
        }

        ValidationErrorsChanged?.Invoke(this, new List<ValidationError>());
        CourseCreated?.Invoke(this, result);
    }
}