using DraftService.DTOs;
using DraftService.Models;
using DraftService.Repositories;
using Monitor;
using System.Diagnostics;

namespace DraftService.Services;

public class DraftService : IDraftService
{
    private readonly IDraftRepository _repository;
    private readonly MonitorService _monitor;

    public DraftService(
        IDraftRepository repository,
        MonitorService monitor)
    {
        _repository = repository;
        _monitor = monitor;
    }

    public async Task<IEnumerable<Draft>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Draft?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Draft> CreateAsync(CreateDraftDto dto)
    {
        using var activity =
            MonitorService.ActivitySource.StartActivity(
                "CreateDraft",
                ActivityKind.Internal);

        var draft = new Draft
        {
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdDraft = await _repository.CreateAsync(draft);

        _monitor.LogInformation(
            "Draft was created. DraftId: {DraftId}",
            createdDraft.Id);

        return createdDraft;
    }

    public async Task<Draft?> UpdateAsync(
        int id,
        UpdateDraftDto dto)
    {
        using var activity =
            MonitorService.ActivitySource.StartActivity(
                "UpdateDraft",
                ActivityKind.Internal);

        var draft = await _repository.GetByIdAsync(id);

        if (draft == null)
        {
            _monitor.LogWarning(
                "Draft not found. DraftId: {DraftId}",
                id);

            return null;
        }

        draft.Title = dto.Title;
        draft.Content = dto.Content;
        draft.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(draft);

        _monitor.LogInformation(
            "Draft was updated. DraftId: {DraftId}",
            draft.Id);

        return draft;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var activity =
            MonitorService.ActivitySource.StartActivity(
                "DeleteDraft",
                ActivityKind.Internal);

        var draft = await _repository.GetByIdAsync(id);

        if (draft == null)
        {
            _monitor.LogWarning(
                "Draft not found. DraftId: {DraftId}",
                id);

            return false;
        }

        await _repository.DeleteAsync(draft);

        _monitor.LogInformation(
            "Draft was deleted. DraftId: {DraftId}",
            id);

        return true;
    }
}
