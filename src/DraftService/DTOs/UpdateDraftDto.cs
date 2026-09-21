using System.ComponentModel.DataAnnotations;

namespace DraftService.DTOs;

public class UpdateDraftDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
