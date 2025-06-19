using Legacy.DataAccess.Models;
using Legacy.DataAccess.Entities;
using Legacy.DataAccess.Records;

namespace Legacy.DataAccess.Mappers;

public static class ShoppingListMapping
{
    public static ShoppingListEntity ToDomain(this ShoppingListDao dao) => new ShoppingListEntity
    {
        Id = new ShoppingListId(dao.Id),
        Name = dao.Name,
        UserId = dao.UserId,
        Tags = dao.Tags?.Select(t => t.ToDomain()).ToList() ?? []
    };

    public static ShoppingListDao ToDao(this ShoppingListEntity domain) => new ShoppingListDao(
        id: domain.Id.Value,
        userId: domain.UserId,
        name: domain.Name,
        items: [],
        tags: domain.Tags?.Select(t => t.ToDao()).ToList() ?? []
    );
}