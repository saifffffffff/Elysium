using Deepgram;
using Deepgram.Clients.Interfaces.v2;
using Deepgram.Models.Authenticate.v1;
using Deepgram.Models.Listen.v2.WebSocket;
using Elysium.Application.Features.Transcription.DTOs;
using Elysium.Application.Features.Transcription.Interfaces;
using Elysium.Application.Features.Transcription.Options;
using Elysium.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Channel = System.Threading.Channels.Channel;

namespace Elysium.Infrastructure.Services;

public sealed class DeepgramTranscriptionProvider : ITranscriptionProvider
{
    private readonly DeepgramOptions _options;

    private static readonly object InitLock = new();
    private static bool _initialized;

    public DeepgramTranscriptionProvider( IOptions<DeepgramOptions> options)
    {
        _options = options.Value;
    }

    private static void EnsureInitialized()
    {
        lock (InitLock)
        {
            if (_initialized)
                return;

            Library.Initialize();
            _initialized = true;
        }
    }

    public async IAsyncEnumerable<TranscriptionSegmentDto> StreamAsync(IAsyncEnumerable<ReadOnlyMemory<byte>> audioChunks, TranscriptionStreamOptions options, [EnumeratorCancellation] CancellationToken ct)
    {
        EnsureInitialized();

        var channel = Channel.CreateUnbounded<TranscriptionSegmentDto>(
                        new UnboundedChannelOptions
                        {
                            SingleReader = true,
                            SingleWriter = true
                        });

        var liveClient = ClientFactory.CreateListenWebSocketClient(_options.ApiKey);//, new DeepgramWsClientOptions { KeepAlive = true });


        await liveClient.Subscribe(
            new EventHandler<ResultResponse>((_, e) =>
            {
                var alternative =
                    e.Channel?.Alternatives?.FirstOrDefault();

                var transcript = alternative?.Transcript;

                if (string.IsNullOrWhiteSpace(transcript))
                    return;

                // We only expose finalized results.
                if (e.IsFinal != true)
                    return;

                channel.Writer.TryWrite(
                    new TranscriptionSegmentDto(
                        transcript,
                        e.Start,
                        e.Start + e.Duration));
            }));

        var connected = await liveClient.Connect(
            new LiveSchema
            {
                Model = options.Model,
                Language = options.Language,

                Encoding = "linear16",
                SampleRate = options.SampleRate,
                Channels = 1,

                EndPointing = options.EndpointingMs.ToString(),
                Punctuate = true,

                InterimResults = false
            });

        if (!connected)
        {
            channel.Writer.TryComplete(new InvalidOperationException("Failed to connect to Deepgram."));

            await liveClient.Stop();

            throw new InvalidOperationException( "Failed to connect to Deepgram.");
        }

        var pumpTask = PumpAudioAsync( liveClient, audioChunks, channel.Writer, ct);

        try
        {
            await foreach (var segment in channel.Reader.ReadAllAsync(ct))
            {
                yield return segment;
            }

            // Wait for the audio pump to finish.
            await pumpTask;
        }
        finally
        {
            // If the consumer stops early/cancels,
            // stop the Deepgram connection.
            if (!pumpTask.IsCompleted)
            {
                try
                {
                    await liveClient.Stop();
                }
                catch
                {
                    // Connection is already being stopped.
                }
            }
        }
    }

    private static async Task PumpAudioAsync( IListenWebSocketClient liveClient, IAsyncEnumerable<ReadOnlyMemory<byte>> audioChunks, ChannelWriter<TranscriptionSegmentDto> writer, CancellationToken ct)
    {
        Exception? error = null;

        try
        {
            await foreach (var chunk in audioChunks.WithCancellation(ct))
            {
                if (chunk.IsEmpty)
                    continue;

                liveClient.Send( chunk.ToArray());
            }

            
            await liveClient.Flush();
            await liveClient.SendFinalize();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Normal cancellation.
        }
        catch (Exception ex)
        {
            error = ex;
        }
        finally
        {
            writer.TryComplete(error);

            try
            {
                await liveClient.Stop();
            }
            catch
            {
                
            }
        }
    }
}