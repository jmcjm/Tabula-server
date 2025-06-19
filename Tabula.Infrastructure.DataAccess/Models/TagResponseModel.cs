using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Models;

public class TagResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TagColor Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
} 