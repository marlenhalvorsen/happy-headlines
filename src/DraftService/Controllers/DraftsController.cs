using DraftService.DTOs;
using DraftService.Models;
using DraftService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DraftService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftsController : ControllerBase
{
    private readonly IDraftService _draftService;

    public DraftsController(IDraftService draftService)
    {
        _draftService = draftService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Draft>>> GetAll()
    {
        var drafts = await _draftService.GetAllAsync();

        return Ok(drafts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Draft>> GetById(int id)
    {
        var draft = await _draftService.GetByIdAsync(id);

        if (draft == null)
            return NotFound();

        return Ok(draft);
    }

    [HttpPost]
    public async Task<ActionResult<Draft>> Create(CreateDraftDto dto)
    {
        var createdDraft = await _draftService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdDraft.Id },
            createdDraft);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Draft>> Update(int id, UpdateDraftDto dto)
    {
        var updatedDraft = await _draftService.UpdateAsync(id, dto);

        if (updatedDraft == null)
            return NotFound();

        return Ok(updatedDraft);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _draftService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
