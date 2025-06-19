using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Mappers;

public static class TagMapping
{
    public static Tag ToDomain(this TagDao dao) => new Tag
    {
        Id = dao.Id,
        Name = dao.Name,
        Color = dao.Color,
        CreatedAt = dao.CreatedAt,
        UserId = dao.UserId
    };

    public static TagDao ToDao(this Tag domain) => new TagDao
    {
        Id = domain.Id,
        Name = domain.Name,
        Color = domain.Color,
        CreatedAt = domain.CreatedAt,
        UserId = domain.UserId
    };
} 