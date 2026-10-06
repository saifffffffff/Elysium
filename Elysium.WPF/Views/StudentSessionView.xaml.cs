using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Views;

/// <summary>
/// Interaction logic for StudentSessionView
/// </summary>
public partial class StudentSessionView : UserControl
{
    private StudentSessionPresenter? _presenter;

    /// <summary>
    /// Raised when the student requests to leave the session
    /// </summary>
    public event EventHandler? LeaveRequested;

    public StudentSessionView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Prepare the view for a session; safe to call more than once
    /// </summary>
    public void Initialize(SessionDto session, JoinSessionResponse joinResponse)
    {
        if (_presenter is null)
        {
            _presenter = new StudentSessionPresenter(
                (IAiChatService)Application.Current.Resources["AiChatService"]!);
            _presenter.Segments.CollectionChanged += Segments_CollectionChanged;
            _presenter.Messages.CollectionChanged += Messages_CollectionChanged;
            _presenter.ChatFailed += Presenter_ChatFailed;
        }

        _presenter.Initialize(session, joinResponse);

        SessionNameText.Text = session.Name;
        TranscriptList.ItemsSource = _presenter.Segments;
        ChatMessagesList.ItemsSource = _presenter.Messages;
    }

    /// <summary>
    /// Append a finalized transcript segment
    /// </summary>
    public void AddSegment(TranscriptionSegment segment)
    {
        _presenter?.AddSegment(segment);
    }

    private void Segments_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        TranscriptScrollViewer.ScrollToEnd();
    }

    private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            ChatScrollViewer.ScrollToEnd();
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Replace && IsChatNearBottom())
            ChatScrollViewer.ScrollToEnd();
    }

    private bool IsChatNearBottom()
    {
        return ChatScrollViewer.ScrollableHeight - ChatScrollViewer.VerticalOffset < 48;
    }

    private void LeaveButton_Click(object sender, RoutedEventArgs e)
    {
        _presenter?.CancelChat();
        LeaveRequested?.Invoke(this, EventArgs.Empty);
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        await SendMessageAsync();
    }

    private async void ChatInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await SendMessageAsync();
            e.Handled = true;
        }
    }

    private async Task SendMessageAsync()
    {
        if (_presenter is null || _presenter.IsSending)
            return;

        SendButton.IsEnabled = false;

        try
        {
            if (await _presenter.SendMessageAsync(ChatInput.Text))
                ChatInput.Clear();
        }
        finally
        {
            SendButton.IsEnabled = true;
        }
    }

    private void Presenter_ChatFailed(object? sender, string message)
    {
        MessageBox.Show(
            message,
            "AI Assistant",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}