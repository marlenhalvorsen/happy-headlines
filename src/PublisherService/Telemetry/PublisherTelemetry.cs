using System.Diagnostics;

namespace PublisherService.Telemetry;

public static class PublisherTelemetry
{
    public const string ServiceName = "PublisherService";

    public static readonly ActivitySource ActivitySource =
        new(ServiceName);
}
