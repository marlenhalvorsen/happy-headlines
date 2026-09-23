using System.ComponentModel.DataAnnotations;

namespace PublisherService.DTOs;

public class PublishArticleDto
{
    [Required]
    public string Title { get; set; }  = string.Empty;
    
    [Required]
    public string Content {  get; set; } = string.Empty;
}