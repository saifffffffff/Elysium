using System.Net.Http;
using System.Text.Json;
using Elysium.WPF.Models.Sessions;

namespace Elysium.WPF.Services;

/// <summary>
/// Session service for API communication
/// </summary>
public class SessionService : ISessionService
{
    private readonly HttpClient _httpClient;
    private string? _lastError;
    private const string ApiBaseUrl = "http://localhost:5129/api/";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SessionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiBaseUrl);
    }

    /// <summary>
    /// Get all sessions belonging to a course
    /// </summary>
    public async Task<List<SessionDto>?> GetSessionsByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            _lastError = null;
            var response = await _httpClient.GetAsync($"sessions/{courseId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<List<SessionDto>>(responseData, JsonOptions);
            }

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Failed to load sessions.");
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