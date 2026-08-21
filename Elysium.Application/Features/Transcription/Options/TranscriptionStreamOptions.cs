namespace Elysium.Application.Features.Transcription.Options;

public class TranscriptionStreamOptions
{
    public string Provider { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string Language { get; set; } = default!;
    public int EndpointingMs { get; set; }
    public int SampleRate { get; set; } 

}