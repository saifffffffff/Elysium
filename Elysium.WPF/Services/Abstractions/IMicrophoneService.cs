namespace Elysium.WPF.Services.Abstractions;

/// <summary>
/// Microphone capture service
/// </summary>
public interface IMicrophoneService
{
    /// <summary>
    /// Whether captured audio is currently being discarded
    /// </summary>
    bool IsMuted { get; set; }

    /// <summary>
    /// Whether the microphone is currently capturing
    /// </summary>
    bool IsCapturing { get; }

    /// <summary>
    /// Raised when capture fails (no device, permissions, etc.)
    /// </summary>
    event EventHandler<string>? Failed;

    /// <summary>
    /// Start capturing audio from the default microphone
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Stop capturing and release the microphone device
    /// </summary>
    void Stop();

    /// <summary>
    /// Enumerate the captured audio chunks (linear16 PCM, 16kHz, mono)
    /// </summary>
    IAsyncEnumerable<byte[]> GetChunks(CancellationToken cancellationToken);
}