using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Mappers;

public static class ShoppingListMapping
{
    public static ShoppingListEntity ToDomain(this ShoppingListDao dao) => new ShoppingListEntity
    {
        Id = dao.Id,
        Name = dao.Name,
        UserId = dao.UserId,
        Tags = dao.Tags?.Select(t => t.ToDomain()).ToList() ?? []
    };

    public static ShoppingListDao ToDao(this ShoppingListEntity domain) => new ShoppingListDao
    {
        Id = domain.Id,
        Name = domain.Name,
        UserId = domain.UserId,
        Tags = domain.Tags?.Select(t => t.ToDao()).ToList() ?? []
    };
}