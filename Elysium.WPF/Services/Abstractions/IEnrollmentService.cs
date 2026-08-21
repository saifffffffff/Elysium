using Elysium.WPF.Models;

namespace Elysium.WPF.Services.Abstractions;

/// <summary>
/// Interface for enrollment service
/// </summary>
public interface IEnrollmentService
{
    /// <summary>
    /// Enroll the current student into a course by code
    /// </summary>
    Task<bool> EnrollStudentAsync(EnrollStudentRequest request);

    /// <summary>
    /// Get the last error message if operation fails
    /// </summary>
    string? GetLastError();
}