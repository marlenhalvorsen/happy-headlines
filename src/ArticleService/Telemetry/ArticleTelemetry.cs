using System.Diagnostics;

namespace ArticleService.Telemetry;

public static class ArticleTelemetry
{
    public const string ServiceName = "ArticleService";

    public static readonly ActivitySource ActivitySource =
        new(ServiceName);
}