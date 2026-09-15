namespace Elysium.Infrastructure.Options;

public class SttDefaults
{
    public const string SectionName = "SpeechToText:Defaults";

    public string Language { get; set; } = default!;
    public int SampleRate { get; set; } = default!;
    public int EndpointingMs { get; set; } = default!;
}
