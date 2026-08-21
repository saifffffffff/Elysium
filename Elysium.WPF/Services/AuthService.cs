using System.Net.Http;
using System.Text;
using System.Text.Json;
using Elysium.WPF.Models;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Services;

/// <summary>
/// Authentication service for API communication
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private string? _lastError;
    private const string ApiBaseUrl = "http://localhost:5129/api/";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

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

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Authentication failed");
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

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Authentication failed");
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
    /// Get the profile of a user by id
    /// </summary>
    public async Task<UserProfile?> GetProfileAsync(int id)
    {
        try
        {
            _lastError = null;
            var response = await _httpClient.GetAsync($"users/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserProfile>(responseData, JsonOptions);
            }

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Authentication failed");
            return null;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Update the profile information of a user
    /// </summary>
    public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
    {
        try
        {
            _lastError = null;
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("users", content);

            if (response.IsSuccessStatusCode)
                return true;

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Authentication failed");
            return false;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Change the username of a user
    /// </summary>
    public async Task<bool> ChangeUsernameAsync(ChangeUsernameRequest request)
    {
        try
        {
            _lastError = null;
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("users/change-username", content);

            if (response.IsSuccessStatusCode)
                return true;

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Authentication failed");
            return false;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Change the password of a user
    /// </summary>
    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            _lastError = null;
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("users/change-password", content);

            if (response.IsSuccessStatusCode)
                return true;

            _lastError = await ApiErrorParser.ExtractErrorMessageAsync(response, "Authentication failed");
            return false;
        }
        catch (Exception ex)
        {
            _lastError = $"Connection error: {ex.Message}";
            return false;
        }
    }
}
