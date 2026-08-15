namespace Elysium.WPF.Models;

/// <summary>
/// Request model for changing a user's username
/// </summary>
public record ChangeUsernameRequest(int Id, string Username);