using System.ComponentModel.DataAnnotations;

namespace DraftService.DTOs;

public class CreateDraftDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
