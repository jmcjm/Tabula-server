using Domain.Enums;
using Domain.Records;

namespace Domain.Entities;

public class TagEntity
{
    public TagId Id { get; private set; }
    public string Name { get; private set; }
    public TagColor Color { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId UserId { get; private set; }
    
    public TagEntity(string name, TagColor color, DateTime createdAt, UserId userId, TagId? id)
    {
        Id = id ?? new TagId(Guid.NewGuid());
        Name = name;
        Color = color;
        CreatedAt = createdAt;
        UserId = userId;
    }
    
    public void UpdateName(string name)
    {
        Name = name;
    }
    
    public void UpdateColor(TagColor color)
    {
        Color = color;
    }
} 