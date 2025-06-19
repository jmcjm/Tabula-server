namespace Tabula.Infrastructure.DataAccess.Models;

public class ItemDao
{
    public Guid Id { get; init; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public Guid ShoppingListId { get; init; }
    
    public ItemDao(Guid id, string productName, int quantity, Guid shoppingListId)
    {
        Id = id;
        ProductName = productName;
        Quantity = quantity;
        ShoppingListId = shoppingListId;
    }
}