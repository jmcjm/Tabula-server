namespace Tabula.Infrastructure.DataAccess.Entities;

public class Item
{
    public Guid? Id { get; }
    public string ProductName { get; }
    public int Quantity { get; }
    public Guid ShoppingListId { get; }

    public Item(string productName, int quantity, Guid shoppingListId, Guid? id)
    {
        Id = id;
        ProductName = productName;
        Quantity = quantity;
        ShoppingListId = shoppingListId;
    }
}