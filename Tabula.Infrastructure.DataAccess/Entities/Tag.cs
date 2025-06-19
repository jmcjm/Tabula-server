using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TagColor Color { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<ShoppingListEntity> ShoppingLists { get; set; } = [];
} 