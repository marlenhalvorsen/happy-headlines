using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Monitor;

public class MonitorService
{
    public TracerProvider TracerProvider { get; }

    public MonitorService(
        string serviceName,
        ActivitySource activitySource)
    {
        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddConsoleExporter()
            .AddZipkinExporter(options =>
            {
                options.Endpoint =
                    new Uri("http://zipkin:9411/api/v2/spans");
            })
            .AddSource(activitySource.Name)
            .SetSampler(new AlwaysOnSampler())
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName))
            .Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq")
            .CreateLogger();
    }

    public void LogInformation(
        string messageTemplate,
        params object[] propertyValues)
    {
        Log.Information(messageTemplate, propertyValues);
    }

    public void LogWarning(
        string messageTemplate,
        params object[] propertyValues)
    {
        Log.Warning(messageTemplate, propertyValues);
    }

    public void LogError(
        string messageTemplate,
        params object[] propertyValues)
    {
        Log.Error(messageTemplate, propertyValues);
    }

    public void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
