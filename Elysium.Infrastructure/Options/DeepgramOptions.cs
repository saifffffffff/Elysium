namespace Elysium.Infrastructure.Options;

public class DeepgramOptions
{
    public const string SectionName = "SpeechToText:Deepgram";

    public string Model { get; set; } = default!;

    public string ApiKey { get; set; } = default!;
}