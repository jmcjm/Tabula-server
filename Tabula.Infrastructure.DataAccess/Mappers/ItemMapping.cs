using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Mappers;

public static class ItemMapping
{
    public static Item ToDomain(this ItemDao dao) => new Item(
        id: dao.Id,
        productName: dao.ProductName,
        quantity: dao.Quantity,
        shoppingListId: dao.ShoppingListId
    );

    public static ItemDao ToDao(this Item domain) => new ItemDao(
        id: domain.Id ?? Guid.NewGuid(),
        productName: domain.ProductName,
        quantity: domain.Quantity,
        shoppingListId: domain.ShoppingListId
    );
}