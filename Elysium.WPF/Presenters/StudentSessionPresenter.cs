using System.Collections.ObjectModel;
using Elysium.WPF.Models.Sessions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the student session view
/// </summary>
public class StudentSessionPresenter
{
    /// <summary>
    /// The live transcript segments for the current session
    /// </summary>
    public ObservableCollection<TranscriptionSegment> Segments { get; } = new();

    /// <summary>
    /// The chat messages for the current session
    /// </summary>
    public ObservableCollection<ChatMessage> Messages { get; } = new();

    /// <summary>
    /// The name of the current session
    /// </summary>
    public string SessionName { get; private set; } = string.Empty;

    /// <summary>
    /// Prepare the presenter for a session; safe to call more than once
    /// </summary>
    public void Initialize(SessionDto session)
    {
        SessionName = session.Name;
        Segments.Clear();
        Messages.Clear();
    }

    /// <summary>
    /// Append a finalized transcript segment
    /// </summary>
    public void AddSegment(TranscriptionSegment segment)
    {
        Segments.Add(segment);
    }

    /// <summary>
    /// Send a chat message; returns whether a message was added
    /// </summary>
    public bool SendMessage(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.Length == 0)
            return false;

        Messages.Add(new ChatMessage("You", trimmed, true));
        return true;
    }
}