using System.Collections.ObjectModel;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Presenters;

/// <summary>
/// Presenter for the student session view
/// </summary>
public class StudentSessionPresenter
{
    private readonly IAiChatService _aiChatService;
    private CancellationTokenSource? _chatCts;
    private bool _isSending;

    public StudentSessionPresenter(IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    public event EventHandler<string>? ChatFailed;

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
    /// The student session id created when the student joined
    /// </summary>
    public int StudentSessionId { get; private set; }

    /// <summary>
    /// Whether an AI answer is currently streaming
    /// </summary>
    public bool IsSending => _isSending;

    /// <summary>
    /// Prepare the presenter for a session; safe to call more than once
    /// </summary>
    public void Initialize(SessionDto session, JoinSessionResponse joinResponse)
    {
        CancelChat();

        SessionName = session.Name;
        StudentSessionId = joinResponse.SessionStudentId;
        Segments.Clear();
        Messages.Clear();

        foreach (var segment in joinResponse.Transcript)
            Segments.Add(segment);
    }

    /// <summary>
    /// Append a finalized transcript segment
    /// </summary>
    public void AddSegment(TranscriptionSegment segment)
    {
        Segments.Add(segment);
    }

    /// <summary>
    /// Cancel an active AI answer stream
    /// </summary>
    public void CancelChat()
    {
        _chatCts?.Cancel();
    }

    /// <summary>
    /// Send a question to the AI and append streamed answer chunks to the chat
    /// </summary>
    public async Task<bool> SendMessageAsync(string text)
    {
        var trimmed = text.Trim();

        if (trimmed.Length == 0 || _isSending)
            return false;

        _isSending = true;

        Messages.Add(new ChatMessage("You", trimmed, true));

        var answerIndex = Messages.Count;
        var currentAnswer = new ChatMessage("AI", string.Empty, false, true);
        Messages.Add(currentAnswer);

        var cts = new CancellationTokenSource();
        _chatCts = cts;

        try
        {
            var request = new AiChatRequest(StudentSessionId, trimmed);

            await foreach (var chunk in _aiChatService.AskAsync(request, cts.Token))
            {
                if (chunk.IsErrorMessage)
                {
                    TryReplaceMessage(
                        answerIndex,
                        currentAnswer,
                        new ChatMessage("Elysium", chunk.Content, false));

                    return true;
                }

                var updatedAnswer = currentAnswer with
                {
                    Text = currentAnswer.Text + chunk.Content,
                    IsPending = false
                };

                if (!TryReplaceMessage(answerIndex, currentAnswer, updatedAnswer))
                    return true;

                currentAnswer = updatedAnswer;
            }

            if (string.IsNullOrWhiteSpace(currentAnswer.Text))
            {
                if (TryRemoveMessage(answerIndex, currentAnswer))
                    ChatFailed?.Invoke(this, "The AI returned an empty response.");
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            TryRemoveMessage(answerIndex, currentAnswer);
            return true;
        }
        catch (Exception ex)
        {
            TryRemoveMessage(answerIndex, currentAnswer);
            ChatFailed?.Invoke(this, ex.Message);
            return true;
        }
        finally
        {
            if (ReferenceEquals(_chatCts, cts))
                _chatCts = null;

            cts.Dispose();
            _isSending = false;
        }
    }

    private bool TryReplaceMessage(int index, ChatMessage expected, ChatMessage replacement)
    {
        if (index < 0 || index >= Messages.Count || !ReferenceEquals(Messages[index], expected))
            return false;

        Messages[index] = replacement;
        return true;
    }

    private bool TryRemoveMessage(int index, ChatMessage expected)
    {
        if (index < 0 || index >= Messages.Count || !ReferenceEquals(Messages[index], expected))
            return false;

        Messages.RemoveAt(index);
        return true;
    }
}