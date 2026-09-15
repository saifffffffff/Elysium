using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Infrastructure.Options;


public class SttOptions
{
    public const string SectionName = "SpeechToText";

    public string ActiveProvider { get; set; } = default!;

    public SttDefaults Defaults { get; set; } = default!;

    public DeepgramOptions Deepgram { get; set;  } = default!;
}
