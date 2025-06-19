using System.ComponentModel.DataAnnotations;
using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Models;

public class CreateTagModel
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public TagColor Color { get; set; }
} 