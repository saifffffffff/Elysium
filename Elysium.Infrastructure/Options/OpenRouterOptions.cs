using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Infrastructure.Options;

public class OpenRouterOptions
{
    public string ApiKey { get; set; }
    public string Model { get; set; }
    public string BaseUrl { get; set; }
}
