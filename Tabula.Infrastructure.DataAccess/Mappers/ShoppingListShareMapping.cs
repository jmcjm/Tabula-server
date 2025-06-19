using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Mappers;

public static class ShoppingListShareMapping
{
    public static ShoppingListShare ToDomain(this ShoppingListShareDao dao) => new ShoppingListShare
    {
        Id = dao.Id,
        ShoppingListId = dao.ShoppingListId,
        OwnerId = dao.OwnerId,
        SharedWithUserId = dao.SharedWithUserId,
        Permission = dao.Permission,
        SharedAt = dao.SharedAt
    };

    public static ShoppingListShareDao ToDao(this ShoppingListShare domain) => new ShoppingListShareDao
    {
        Id = domain.Id,
        ShoppingListId = domain.ShoppingListId,
        OwnerId = domain.OwnerId,
        SharedWithUserId = domain.SharedWithUserId,
        Permission = domain.Permission,
        SharedAt = domain.SharedAt
    };
} 