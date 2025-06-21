using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Presentation.DTOs.Requests;

public class CreateTagRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public TagColor Color { get; init; }
} 