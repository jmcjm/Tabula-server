namespace Infrastructure.DbModels;

public class ShoppingListDbModel
{
    public Guid Id { get; set; }
    public required string Name { get;  set; }
    public required string UserId { get; set; }
    public ICollection<ItemDbModel> Items { get; set; } = [];
    public ICollection<TagDbModel> Tags { get; set; } = [];
}