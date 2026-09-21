using DraftService.Data;
using DraftService.Models;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Repositories;

public class DraftRepository : IDraftRepository
{
    private readonly DraftDbContext _context;

    public DraftRepository(DraftDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Draft>> GetAllAsync()
    {
        return await _context.Drafts.ToListAsync();
    }

    public async Task<Draft?> GetByIdAsync(int id)
    {
        return await _context.Drafts.FindAsync(id);
    }

    public async Task<Draft> CreateAsync(Draft draft)
    {
        _context.Drafts.Add(draft);
        await _context.SaveChangesAsync();

        return draft;
    }

    public async Task UpdateAsync(Draft draft)
    {
        _context.Drafts.Update(draft);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Draft draft)
    {
        _context.Drafts.Remove(draft);
        await _context.SaveChangesAsync();
    }
}
