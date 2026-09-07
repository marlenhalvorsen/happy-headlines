using Microsoft.AspNetCore.Mvc;
using ProfanityService.Services;

namespace ProfanityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfanityController : ControllerBase
{
    private readonly ProfanityFilterService _profanityFilterService;

    public ProfanityController(ProfanityFilterService  profanityFilterService)
    {
        _profanityFilterService = profanityFilterService;
    }
    
    [HttpPost("check")]
    public async Task<IActionResult> CheckProfanity([FromBody] string text)
    {
        var containsProfanity = await _profanityFilterService.ContainsProfanityAsync(text);
            
        return Ok(new{containsProfanity});
    }
}