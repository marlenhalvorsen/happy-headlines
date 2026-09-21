using DraftService.DTOs;
using DraftService.Models;

namespace DraftService.Services;

public interface IDraftService
{
    Task<IEnumerable<Draft>> GetAllAsync();
    Task<Draft?> GetByIdAsync(int id);
    Task<Draft> CreateAsync(CreateDraftDto dto);
    Task<Draft?> UpdateAsync(int id, UpdateDraftDto dto);
    Task<bool> DeleteAsync(int id);
}
