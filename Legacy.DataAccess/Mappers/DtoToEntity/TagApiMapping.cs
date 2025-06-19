using Legacy.DataAccess.Models;
using Legacy.DataAccess.Entities;

namespace Legacy.DataAccess.Mappers;

public static class TagApiMapping
{
    public static TagResponseModel ToResponseModel(this TagEntity tagEntity) => new TagResponseModel
    {
        Id = tagEntity.Id,
        Name = tagEntity.Name,
        Color = tagEntity.Color,
        CreatedAt = tagEntity.CreatedAt,
        UserId = tagEntity.UserId
    };

    public static TagEntity ToEntity(this CreateTagModel model, string userId) => new TagEntity
    {
        Name = model.Name,
        Color = model.Color,
        UserId = userId,
        CreatedAt = DateTime.UtcNow
    };
} 