using System.Net.Http;
using System.Text;
using System.Text.Json;
using Elysium.WPF.Models;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Services;

/// <summary>
/// Enrollment service for API communication
/// </summary>
public class EnrollmentService : IEnrollmentService
{
    private readonly HttpClient _httpClient;
    private string? _lastError;
    private const string ApiBaseUrl = "http://localhost:5129/api/";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public EnrollmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiBaseUrl);
    }

    /// <summary>
    /// Enroll the current student into a course by code
    /// </summary>
    public async Task<bool> EnrollStudentAsync(EnrollStudentRequest request)
    {
        try
        {
            _lastError = null;
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("enrollments", content);

            if (response.IsSuccessStatusCode)
                return true;

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Failed to enroll in course");
            return false;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return false;
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