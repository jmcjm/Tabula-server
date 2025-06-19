using System.ComponentModel.DataAnnotations;
using Legacy.DataAccess.Enums;

namespace Legacy.DataAccess.DTOs.Requests;

public class CreateTagModel
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public TagColor Color { get; set; }
} 