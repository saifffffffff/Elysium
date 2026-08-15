using System.Net.Http;
using System.Text.Json;

namespace Elysium.WPF.Services;

/// <summary>
/// Shared helper for extracting error messages from API error responses
/// </summary>
public static class ApiErrorParser
{
    /// <summary>
    /// Extract the error message from an API error response
    /// </summary>
    public static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, string fallback = "Request failed")
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();

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
                    return errorElement.GetString() ?? fallback;
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