using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using NewsletterService.Telemetry;

namespace NewsletterService.Clients;

public class ArticleClient
{
    private readonly HttpClient _httpClient;

    public ArticleClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> FetchArticleAsync(
        string region,
        int id)
    {
        using var activity =
            NewsletterTelemetry.ActivitySource.StartActivity(
                "Request Article",
                ActivityKind.Client);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/article/{region}/{id}");

        var propagator = new TraceContextPropagator();

        propagator.Inject(
            new PropagationContext(
                activity?.Context ?? default,
                Baggage.Current),
            request,
            (httpRequest, key, value) =>
            {
                httpRequest.Headers.TryAddWithoutValidation(
                    key,
                    value);
            });

        var response =
            await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync();
    }
}
