using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Mappers;

public static class TagApiMapping
{
    public static TagResponseModel ToResponseModel(this Tag tag) => new TagResponseModel
    {
        Id = tag.Id,
        Name = tag.Name,
        Color = tag.Color,
        CreatedAt = tag.CreatedAt,
        UserId = tag.UserId
    };

    public static Tag ToEntity(this CreateTagModel model, string userId) => new Tag
    {
        Name = model.Name,
        Color = model.Color,
        UserId = userId,
        CreatedAt = DateTime.UtcNow
    };
} 