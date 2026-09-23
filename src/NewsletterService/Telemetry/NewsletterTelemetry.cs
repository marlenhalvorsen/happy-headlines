using System.Diagnostics;

namespace NewsletterService.Telemetry;

public static class NewsletterTelemetry
{
    public const string ServiceName = "NewsletterService";

    public static readonly ActivitySource ActivitySource =
        new(ServiceName);
}
