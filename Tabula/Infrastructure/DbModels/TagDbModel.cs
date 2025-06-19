namespace Infrastructure.DbModels;

public class TagDbModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string UserId { get; set; }
    public ICollection<ShoppingListDbModel> ShoppingLists { get; set; } = [];
}