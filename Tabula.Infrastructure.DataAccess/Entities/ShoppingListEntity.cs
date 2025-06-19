namespace Tabula.Infrastructure.DataAccess.Entities;

public class ShoppingListEntity
{
    public required string Name { get; set; }
    public required string UserId { get; set; }
    
    // Navigation properties
    public ICollection<Tag> Tags { get; set; } = [];
}