using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;

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
    public void Initialize(SessionDto session)
    {
        if (_presenter is null)
        {
            _presenter = new StudentSessionPresenter();
            _presenter.Segments.CollectionChanged += Segments_CollectionChanged;
            _presenter.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        _presenter.Initialize(session);

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
        ChatScrollViewer.ScrollToEnd();
    }

    private void LeaveButton_Click(object sender, RoutedEventArgs e)
    {
        LeaveRequested?.Invoke(this, EventArgs.Empty);
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        SendMessage();
    }

    private void ChatInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SendMessage();
            e.Handled = true;
        }
    }

    private void SendMessage()
    {
        if (_presenter is null)
            return;

        if (_presenter.SendMessage(ChatInput.Text))
            ChatInput.Clear();
    }
}