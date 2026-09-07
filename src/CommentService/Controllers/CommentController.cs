using CommentService.Data;
using CommentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentDbContext _context;

    public CommentController(CommentDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }
}