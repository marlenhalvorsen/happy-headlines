using Microsoft.AspNetCore.Mvc;
using PublisherService.DTOs;
using PublisherService.Services;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController : ControllerBase
{
    private readonly IPublishService _publishService;

    public PublisherController(IPublishService publishService)
    {
        _publishService = publishService;
    }

    [HttpPost]
    public async Task<IActionResult> Publish(PublishArticleDto dto)
    {
        await _publishService.PublishArticleAsync(dto);

        return Accepted();
    }
}
