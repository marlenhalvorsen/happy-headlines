using System.Diagnostics;
using ArticleService.Models;
using ArticleService.Telemetry;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Context.Propagation;

namespace ArticleService;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly Services.ArticleService _articleService;

    public ArticleController(Services.ArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpPost("{region}")]
    public async Task<IActionResult> CreateArticle(
        string region,
        [FromBody] Article article)
    {
        var createdArticle =
            await _articleService.CreateArticle(region, article);

        return Ok(createdArticle);
    }

    [HttpGet("{region}/{id}")]
    public async Task<IActionResult> FetchArticle(string region, int id)
    {
        var propagator = new TraceContextPropagator();

        // Extract trace context sent by NewsletterService.
        var propagationContext = propagator.Extract(
            default,
            Request,
            (request, key) =>
            {
                if (request.Headers.TryGetValue(key, out var values))
                {
                    return values.ToArray();
                }

                return Array.Empty<string>();
            });

        // Continue the same distributed trace.
        using var activity =
            ArticleTelemetry.ActivitySource.StartActivity(
                "Fetch Article",
                ActivityKind.Server,
                propagationContext.ActivityContext);

        var article =
            await _articleService.FetchArticle(region, id);

        if (article == null)
            return NotFound();

        return Ok(article);
    }

    [HttpDelete("{region}/{id}")]
    public async Task<IActionResult> DeleteArticle(string region, int id)
    {
        var deleted =
            await _articleService.DeleteArticle(region, id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{region}/{id}")]
    public async Task<IActionResult> UpdateArticle(
        string region,
        int id,
        [FromBody] Article article)
    {
        var updatedArticle =
            await _articleService.UpdateArticle(region, id, article);

        if (updatedArticle == null)
            return NotFound();

        return Ok(updatedArticle);
    }
}