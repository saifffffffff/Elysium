namespace Elysium.WPF.Models;

/// <summary>
/// Request model for changing a user's password
/// </summary>
public record ChangePasswordRequest(int Id, string CurrentPassword, string NewPassword);