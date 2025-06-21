using Domain.Errors;
using Domain.Records;
using Domain.Validations;
using ErrorOr;

namespace Domain.Entities;

public class ItemEntity
{
    public ItemId Id { get; private set; }
    public string ProductName { get; private set; }
    public ushort Quantity { get; private set; }
    public bool Bought { get; private set; }
    public ShoppingListId ShoppingListId { get; private set; }
    
    private const byte MaxProductNameLength = 50;
    
    private ItemEntity(string productName, ushort quantity, bool bought, ShoppingListId shoppingListId, ItemId? id)
    {
        Id = id ?? new ItemId(Guid.NewGuid());
        ProductName = productName;
        Quantity = quantity;
        Bought = bought;
        ShoppingListId = shoppingListId;
    }
    
    public static ErrorOr<ItemEntity> Create(string productName, ushort quantity, bool bought, ShoppingListId shoppingListId)
    {
        var productNameValidation = DomainValidators.NameValidator(productName, MaxProductNameLength);
        
        if (productNameValidation.IsError) return productNameValidation.Errors;

        var entity = new ItemEntity(
            productName: productName.Trim(),
            quantity: quantity,
            bought: bought,
            shoppingListId: shoppingListId,
            id: null);

        return entity;
    }
    
    public static ErrorOr<ItemEntity> Restore(ItemId id, string productName, ushort quantity, bool bought, ShoppingListId shoppingListId) 
    {
        var productNameValidation = DomainValidators.NameValidator(productName, MaxProductNameLength);
        
        if (productNameValidation.IsError) return productNameValidation.Errors;

        var entity = new ItemEntity(
            productName: productName.Trim(),
            quantity: quantity,
            bought: bought,
            shoppingListId: shoppingListId,
            id: id);

        return entity;
    }
    
    public void UpdateQuantity(ushort quantity)
    {
        Quantity = quantity;
    }
    
    public void UpdateBought(bool bought)
    {
        Bought = bought;
    }
    
    public ErrorOr<Success> UpdateName(string productName)
    {
        var validation = DomainValidators.NameValidator(productName, MaxProductNameLength);
        
        if (validation.IsError) return validation.Errors;

        ProductName = productName.Trim();
        return Result.Success;
    }
}