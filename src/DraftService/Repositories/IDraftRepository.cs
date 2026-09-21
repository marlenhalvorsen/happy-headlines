using DraftService.Models;

namespace DraftService.Repositories;

public interface IDraftRepository
{
    Task<IEnumerable<Draft>> GetAllAsync();
    Task<Draft?> GetByIdAsync(int id);
    Task<Draft> CreateAsync(Draft draft);
    Task UpdateAsync(Draft draft);
    Task DeleteAsync(Draft draft);
}
