using Polly.CircuitBreaker;
using CommentService.Models;
using CommentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentManager _commentManager;

    public CommentController(CommentManager commentManager)
    {
        _commentManager = commentManager;
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment(Comment comment)
    {
        try
        {
            var createdComment =
                await _commentManager.CreateCommentAsync(comment);
    
            if (createdComment == null)
            {
                return BadRequest("Comment contains profanity.");
            }
    
            return Ok(createdComment);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Profanity service is currently unavailable.");
        }
        catch (BrokenCircuitException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Profanity service is currently unavailable.");
        }
    }
}
