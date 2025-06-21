using Domain.Enums;
using Domain.Records;
using ErrorOr;
using Domain.Errors;
using Domain.Validations;

namespace Domain.Entities;

public class TagEntity
{
    public TagId Id { get; private set; }
    public string Name { get; private set; }
    public TagColor Color { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId UserId { get; private set; }
    
    private const byte MaxNameLength = 50;

    private TagEntity(string name, TagColor color, DateTime createdAt, UserId userId, TagId? id)
    {
        Id = id ?? new TagId(Guid.NewGuid());
        Name = name;
        Color = color;
        CreatedAt = createdAt;
        UserId = userId;
    }

    public static ErrorOr<TagEntity> Create(string name, TagColor color, UserId userId, DateTime? createdAt = null)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        var entity = new TagEntity(
            name: name.Trim(),
            color: color,
            createdAt: createdAt ?? DateTime.UtcNow,
            userId: userId,
            id: null);

        return entity;
    }

    public static ErrorOr<TagEntity> Restore(TagId id, string name, TagColor color, DateTime createdAt, UserId userId)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        var entity = new TagEntity(
            name: name.Trim(),
            color: color,
            createdAt: createdAt,
            userId: userId,
            id: id);

        return entity;
    }

    public ErrorOr<Success> UpdateName(string name)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        Name = name.Trim();
        return Result.Success;
    }

    public void UpdateColor(TagColor color)
    {
        Color = color;
    }
} 