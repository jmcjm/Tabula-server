using Legacy.DataAccess.Models;
using Legacy.DataAccess.Entities;
using Legacy.DataAccess.Records;

namespace Legacy.DataAccess.Mappers;

public static class ShoppingListShareMapping
{
    public static ShoppingListShareEntity ToDomain(this ShoppingListShareDao dao) => new ShoppingListShareEntity
    {
        Id = new ShoppingListShareId(dao.Id),
        ShoppingListId = new ShoppingListId(dao.ShoppingListId),
        OwnerId = dao.OwnerId,
        SharedWithUserId = dao.SharedWithUserId,
        Permission = dao.Permission,
        SharedAt = dao.SharedAt
    };

    public static ShoppingListShareDao ToDao(this ShoppingListShareEntity domain) => new ShoppingListShareDao
    {
        Id = domain.Id.Value,
        ShoppingListId = domain.ShoppingListId.Value,
        OwnerId = domain.OwnerId,
        SharedWithUserId = domain.SharedWithUserId,
        Permission = domain.Permission,
        SharedAt = domain.SharedAt
    };
} 