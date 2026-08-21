using System.Net.Http;
using System.Text;
using System.Text.Json;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Services;

/// <summary>
/// Course service for API communication
/// </summary>
public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;
    private string? _lastError;
    private const string ApiBaseUrl = "http://localhost:5129/api/";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiBaseUrl);
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    public async Task<CreateCourseResponse?> CreateAsync(CreateCourseRequest request)
    {
        try
        {
            _lastError = null;
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("courses", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CreateCourseResponse>(responseData, JsonOptions);
            }

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Failed to create course");
            return null;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Get all courses belonging to a teacher
    /// </summary>
    public async Task<IReadOnlyList<CourseDto>?> GetCoursesByTeacherAsync(int teacherId)
    {
        try
        {
            _lastError = null;
            var response = await _httpClient.GetAsync($"courses/{teacherId}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseDto>>(responseData, JsonOptions);
            }

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Failed to load courses.");
            return null;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Get all courses a student is enrolled in
    /// </summary>
    public async Task<IReadOnlyList<CourseDto>?> GetCoursesByStudentAsync(int studentId)
    {
        try
        {
            _lastError = null;
            var response = await _httpClient.GetAsync($"courses/student/{studentId}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseDto>>(responseData, JsonOptions);
            }

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Failed to load courses.");
            return null;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Get the last error message
    /// </summary>
    public string? GetLastError()
    {
        return _lastError;
    }
}