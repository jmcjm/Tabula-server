namespace Infrastructure.DbModels;

public class ItemDbModel
{
    public Guid Id { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public bool Bought { get; set; }
    public Guid ShoppingListId { get; set; }
}