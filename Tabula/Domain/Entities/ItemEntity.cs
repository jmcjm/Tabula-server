using Domain.Records;

namespace Domain.Entities;

public class ItemEntity
{
    public ItemId Id { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public bool Bought { get; private set; }
    public ShoppingListId ShoppingListId { get; private set; }
    
    public ItemEntity(string productName, int quantity, bool bought, ShoppingListId shoppingListId, ItemId? id)
    {
        Id = id ?? new ItemId(Guid.NewGuid());
        ProductName = productName;
        Quantity = quantity;
        Bought = bought;
        ShoppingListId = shoppingListId;
    }
    
    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
    
    public void UpdateBought(bool bought)
    {
        Bought = bought;
    }
    
    public void UpdateName(string productName)
    {
        ProductName = productName;
    }
}