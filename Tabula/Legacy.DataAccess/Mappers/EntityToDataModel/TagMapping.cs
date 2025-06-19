using Legacy.DataAccess.Models;
using Legacy.DataAccess.Entities;
using Legacy.DataAccess.Records;

namespace Legacy.DataAccess.Mappers;

public static class TagMapping
{
    public static TagEntity ToDomain(this TagDao dao) => new TagEntity
    {
        Id = new TagId(dao.Id),
        Name = dao.Name,
        Color = dao.Color,
        CreatedAt = dao.CreatedAt,
        UserId = dao.UserId
    };

    public static TagDao ToDao(this TagEntity domain) => new TagDao
    {
        Id = domain.Id.Value,
        Name = domain.Name,
        Color = domain.Color,
        CreatedAt = domain.CreatedAt,
        UserId = domain.UserId
    };
} 