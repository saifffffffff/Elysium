using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Elysium.WPF.Services.Abstractions;
using NAudio.Wave;
using Channel = System.Threading.Channels.Channel;

namespace Elysium.WPF.Services;

/// <summary>
/// Microphone capture using NAudio, writing raw PCM as a WAV file
/// </summary>
public class MicrophoneService : IMicrophoneService
{
    private const int SampleRate = 16000;
    private const int BitsPerSample = 16;
    private const int Channels = 1;

    private static readonly string OutputDirectory = Path.Combine(Path.GetTempPath(), "Elysium");

    private WaveInEvent? _waveIn;
    private FileStream? _fileStream;
    private string? _outputPath;
    private Channel<byte[]>? _chunkChannel;
    private readonly object _fileLock = new();

    public bool IsMuted { get; set; }

    public bool IsCapturing => _waveIn is not null;

    public event EventHandler<string>? Failed;

    public Task StartAsync()
    {
        Stop();

        try
        {
            Directory.CreateDirectory(OutputDirectory);
            _outputPath = Path.Combine(OutputDirectory, $"mic_capture_{DateTime.Now:yyyyMMdd_HHmmss}.wav");

            _fileStream = new FileStream(_outputPath, FileMode.Create, FileAccess.ReadWrite);
            WriteWavHeader(_fileStream);

            _chunkChannel = Channel.CreateUnbounded<byte[]>(
                new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

            _waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(SampleRate, BitsPerSample, Channels),
                BufferMilliseconds = 100
            };
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;

            _waveIn.StartRecording();
        }
        catch (Exception ex)
        {
            Cleanup();
            Failed?.Invoke(this, ex.Message);
        }

        return Task.CompletedTask;
    }

    public void Stop()
    {
        if (_waveIn is null && _fileStream is null && _chunkChannel is null)
            return;

        try
        {
            _waveIn?.StopRecording();
        }
        catch
        {
        }

        _waveIn?.Dispose();
        _waveIn = null;

        _chunkChannel?.Writer.TryComplete();
        _chunkChannel = null;

        lock (_fileLock)
        {
            if (_fileStream is not null)
            {
                PatchWavHeader(_fileStream);
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }

    public async IAsyncEnumerable<byte[]> GetChunks([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var channel = _chunkChannel;
        if (channel is null)
            yield break;

        await foreach (var chunk in channel.Reader.ReadAllAsync(cancellationToken))
            yield return chunk;
    }

    private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
    {
        if (IsMuted)
            return;

        _chunkChannel?.Writer.TryWrite(e.Buffer.AsSpan(0, e.BytesRecorded).ToArray());

        lock (_fileLock)
        {
            if (_fileStream is null)
                return;

            _fileStream.Write(e.Buffer, 0, e.BytesRecorded);
        }
    }

    private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
    {
        if (e.Exception is not null && _waveIn is not null)
            Failed?.Invoke(this, e.Exception.Message);
    }

    private static void WriteWavHeader(Stream stream)
    {
        stream.Write(Encoding.ASCII.GetBytes("RIFF"));
        stream.Write(BitConverter.GetBytes(36));
        stream.Write(Encoding.ASCII.GetBytes("WAVE"));

        stream.Write(Encoding.ASCII.GetBytes("fmt "));
        stream.Write(BitConverter.GetBytes(16));
        stream.Write(BitConverter.GetBytes((short)1));
        stream.Write(BitConverter.GetBytes((short)Channels));
        stream.Write(BitConverter.GetBytes(SampleRate));
        stream.Write(BitConverter.GetBytes(SampleRate * Channels * BitsPerSample / 8));
        stream.Write(BitConverter.GetBytes((short)(Channels * BitsPerSample / 8)));
        stream.Write(BitConverter.GetBytes((short)BitsPerSample));

        stream.Write(Encoding.ASCII.GetBytes("data"));
        stream.Write(BitConverter.GetBytes(0));
    }

    private static void PatchWavHeader(FileStream stream)
    {
        var dataLength = stream.Length - 44;
        if (dataLength < 0)
            return;

        stream.Seek(4, SeekOrigin.Begin);
        stream.Write(BitConverter.GetBytes(36 + (int)dataLength));

        stream.Seek(40, SeekOrigin.Begin);
        stream.Write(BitConverter.GetBytes((int)dataLength));

        stream.Flush();
    }

    private void Cleanup()
    {
        try
        {
            _waveIn?.Dispose();
        }
        catch
        {
        }

        _waveIn = null;

        _chunkChannel?.Writer.TryComplete();
        _chunkChannel = null;

        lock (_fileLock)
        {
            try
            {
                _fileStream?.Dispose();
            }
            catch
            {
            }

            _fileStream = null;
        }
    }
}