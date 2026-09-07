using CommentService.Clients;
using CommentService.Data;
using CommentService.Models;

namespace CommentService.Services;

public class CommentManager
{
    private readonly CommentDbContext _context;
    private readonly ProfanityClient _profanityClient;

    public CommentManager(
        CommentDbContext context,
        ProfanityClient profanityClient)
    {
        _context = context;
        _profanityClient = profanityClient;
    }

    public async Task<Comment?> CreateCommentAsync(Comment comment)
    {
        var containsProfanity =
            await _profanityClient.ContainsProfanityAsync(comment.Text);

        if (containsProfanity)
        {
            return null;
        }

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return comment;
    }
}
