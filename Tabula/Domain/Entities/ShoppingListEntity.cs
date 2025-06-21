using Domain.Errors;
using Domain.Records;
using Domain.Validations;
using ErrorOr;

namespace Domain.Entities;

public class ShoppingListEntity
{
    public ShoppingListId Id { get; private set; }
    public string Name { get; private set; }
    public UserId UserId { get; private set; }
    
    public IReadOnlyList<TagEntity> Tags => _tags.AsReadOnly();
    private readonly List<TagEntity> _tags = [];
    public IReadOnlyList<ItemEntity> Items => _items.AsReadOnly();
    private readonly List<ItemEntity> _items = [];
    
    private const int MaxNameLength = 50;
    
    private ShoppingListEntity(UserId userId, string name, ShoppingListId? id)
    {
        Id = id ?? new ShoppingListId(Guid.NewGuid());
        UserId = userId;
        Name = name;
    }
    
    public static ErrorOr<ShoppingListEntity> Create(UserId userId, string name)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        var entity = new ShoppingListEntity(
            userId: userId,
            name: name.Trim(),
            id: null);

        return entity;
    }
    
    public static ErrorOr<ShoppingListEntity> Restore(ShoppingListId id, string name, UserId userId)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        var entity = new ShoppingListEntity(
            userId: userId,
            name: name.Trim(),
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
    
    public void AddItems(List<ItemEntity> items)
    {
        _items.AddRange(items);
    }
    
    public void AddTags(List<TagEntity> tags)
    {
        _tags.AddRange(tags);
    }
}