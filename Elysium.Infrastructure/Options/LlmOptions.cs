namespace Elysium.Infrastructure.Options;

public class LlmOptions
{
    public const string SectionName = "AiChat";
    public string ActiveProvider { get; set; }
    public LlmDefaults Defaults { get; set; }
    public OpenRouterOptions OpenRouter { get; set; }
}
