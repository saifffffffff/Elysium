using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Elysium.WPF.Models.Sessions;
using Elysium.WPF.Presenters;
using Elysium.WPF.Services;
using Elysium.WPF.Services.Abstractions;

namespace Elysium.WPF.Views;

/// <summary>
/// Interaction logic for TeacherSessionView
/// </summary>
public partial class TeacherSessionView : UserControl
{
    private TeacherSessionPresenter? _presenter;

    /// <summary>
    /// Raised when the teacher requests to end the session
    /// </summary>
    public event EventHandler? EndSessionRequested;

    public TeacherSessionView()
    {
        InitializeComponent();
        Unloaded += TeacherSessionView_Unloaded;
    }

    /// <summary>
    /// Prepare the view for a session; safe to call more than once
    /// </summary>
    public void Initialize(int sessionId, string name)
    {
        if (_presenter is null)
        {
            _presenter = new TeacherSessionPresenter(
                (ISessionService)Application.Current.Resources["SessionService"]!,
                (ISessionHubService)Application.Current.Resources["SessionHubService"]!,
                (IMicrophoneService)Application.Current.Resources["MicrophoneService"]!
            );
            _presenter.SessionEnded += Presenter_SessionEnded;
            _presenter.EndSessionFailed += Presenter_EndSessionFailed;
            _presenter.MuteStateChanged += Presenter_MuteStateChanged;
            _presenter.MicFailed += Presenter_MicFailed;
            _presenter.Segments.CollectionChanged += Segments_CollectionChanged;
        }

        _presenter.Initialize(sessionId, name);

        SessionNameText.Text = name;
        TranscriptList.ItemsSource = _presenter.Segments;
        ApplyMuteState();
    }

    private void TeacherSessionView_Unloaded(object? sender, RoutedEventArgs e)
    {
        _presenter?.Stop();
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

    private async void EndSessionButton_Click(object sender, RoutedEventArgs e)
    {
        ShowEndConfirmation();
    }

    private void CancelEndButton_Click(object sender, RoutedEventArgs e)
    {
        HideEndConfirmation();
    }

    private async void ConfirmEndButton_Click(object sender, RoutedEventArgs e)
    {
        HideEndConfirmation();
        await TryEndSessionAsync();
    }

    /// <summary>
    /// End the current session; returns true when the session was ended successfully
    /// </summary>
    public async Task<bool> TryEndSessionAsync()
    {
        if (_presenter is null)
            return false;

        EndSessionButton.IsEnabled = false;
        try
        {
            return await _presenter.HandleEndSessionAsync();
        }
        finally
        {
            EndSessionButton.IsEnabled = true;
        }
    }

    private void ShowEndConfirmation()
    {
        EndConfirmOverlay.Visibility = Visibility.Visible;
        EndConfirmOverlay.Opacity = 0;
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        EndConfirmOverlay.BeginAnimation(OpacityProperty, fadeIn);
    }

    private void HideEndConfirmation()
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
        fadeOut.Completed += (_, _) => EndConfirmOverlay.Visibility = Visibility.Collapsed;
        EndConfirmOverlay.BeginAnimation(OpacityProperty, fadeOut);
    }

    private void Presenter_SessionEnded(object? sender, EventArgs e)
    {
        EndSessionRequested?.Invoke(this, EventArgs.Empty);
    }

    private void Presenter_EndSessionFailed(object? sender, string message)
    {
        MessageBox.Show(message, "Elysium", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void Presenter_MicFailed(object? sender, string message)
    {
        MessageBox.Show($"The microphone could not be started.\n\n{message}",
                        "Microphone Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void MicToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _presenter?.ToggleMute();
    }

    private void Presenter_MuteStateChanged(object? sender, EventArgs e)
    {
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        var isMuted = _presenter?.IsMuted ?? false;

        MicOnIcon.Visibility = isMuted ? Visibility.Collapsed : Visibility.Visible;
        MicOffIcon.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;
        MicStatusText.Text = isMuted ? "Muted" : "Listening";
    }
}