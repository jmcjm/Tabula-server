using Legacy.DataAccess.Models;
using Legacy.DataAccess.Entities;
using Legacy.DataAccess.Records;

namespace Legacy.DataAccess.Mappers;

public static class ItemMapping
{
    public static ItemEntity ToDomain(this ItemDao dao) => new ItemEntity(
        id: new ItemId(dao.Id),
        productName: dao.ProductName,
        quantity: dao.Quantity,
        shoppingListId: new ShoppingListId(dao.ShoppingListId)
    );

    public static ItemDao ToDao(this ItemEntity domain) => new ItemDao(
        id: domain.Id.Value,
        productName: domain.ProductName,
        quantity: domain.Quantity,
        shoppingListId: domain.ShoppingListId.Value
    );
}