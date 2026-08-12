using System.Net.Http;
using System.Text;
using System.Text.Json;
using Elysium.WPF.Models;

namespace Elysium.WPF.Services;

/// <summary>
/// Authentication service for API communication
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private string? _lastError;
    private const string ApiBaseUrl = "http://localhost:5129/api/";

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiBaseUrl);
    }

    /// <summary>
    /// Authenticate user with username and password
    /// </summary>
    public async Task<AuthResponse?> SignInAsync(string username, string password)
    {
        try
        {
            _lastError = null;
            var request = new SignInRequest(username, password);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users/signin", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseData, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return authResponse;
            }

            _lastError = await ExtractErrorMessage(response);
            return null;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    public async Task<AuthResponse?> SignUpAsync(CreateUserRequest request)
    {
        try
        {
            _lastError = null;
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return authResponse;
            }

            _lastError = await ExtractErrorMessage(response);
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

    /// <summary>
    /// Extract error message from API response
    /// </summary>
    private async Task<string> ExtractErrorMessage(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();

            // Try to parse as JSON error response
            using (var doc = JsonDocument.Parse(content))
            {
                var root = doc.RootElement;

                // API returns a top-level array of error objects
                if (root.ValueKind == JsonValueKind.Array)
                {
                    var errors = new List<string>();
                    foreach (var error in root.EnumerateArray())
                    {
                        if (error.TryGetProperty("message", out var msgElement))
                        {
                            errors.Add(msgElement.GetString() ?? "Unknown error");
                        }
                        else if (error.ValueKind == JsonValueKind.String)
                        {
                            errors.Add(error.GetString() ?? "Unknown error");
                        }
                    }
                    if (errors.Count > 0)
                        return string.Join("; ", errors);
                }

                if (root.TryGetProperty("errors", out var errorsElement))
                {
                    if (errorsElement.ValueKind == JsonValueKind.Array)
                    {
                        var errors = new List<string>();
                        foreach (var error in errorsElement.EnumerateArray())
                        {
                            if (error.TryGetProperty("message", out var msgElement))
                            {
                                errors.Add(msgElement.GetString() ?? "Unknown error");
                            }
                            else if (error.ValueKind == JsonValueKind.String)
                            {
                                errors.Add(error.GetString() ?? "Unknown error");
                            }
                        }
                        if (errors.Count > 0)
                            return string.Join("; ", errors);
                    }
                }

                // Try alternate error format
                if (root.TryGetProperty("error", out var errorElement))
                {
                    return errorElement.GetString() ?? "Authentication failed";
                }
            }

            return $"Error: {response.StatusCode}";
        }
        catch
        {
            return $"Error: {response.StatusCode}";
        }
    }
}
