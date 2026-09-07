using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;

namespace ProfanityService.Services;

public class ProfanityFilterService
{
    private readonly ProfanityDbContext _context;

    public ProfanityFilterService(ProfanityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ContainsProfanityAsync(string text)
    {
        var profanityWords = await _context.ProfanityWords
            .Select(p => p.Word) // Could be cached under high traffic to avoid unnecessary database calls.
            .ToListAsync();

        return profanityWords.Any(word =>
            text.Contains(word, StringComparison.OrdinalIgnoreCase));
    } 
}