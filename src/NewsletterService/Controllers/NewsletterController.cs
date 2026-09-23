using Microsoft.AspNetCore.Mvc;
using NewsletterService.Clients;

namespace NewsletterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly ArticleClient _articleClient;

    public NewsletterController(ArticleClient articleClient)
    {
        _articleClient = articleClient;
    }

    [HttpGet("daily/{region}/{id}")]
    public async Task<IActionResult> GetDailyArticle(
        string region,
        int id)
    {
        var article =
            await _articleClient.FetchArticleAsync(region, id);

        if (article == null)
            return NotFound();

        return Content(article, "application/json");
    }
}
