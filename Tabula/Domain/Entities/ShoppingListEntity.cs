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
    
    private ShoppingListEntity(UserId userId, string name, ShoppingListId? id, ICollection<ItemEntity>? items, ICollection<TagEntity>? tags)
    {
        Id = id ?? new ShoppingListId(Guid.NewGuid());
        UserId = userId;
        Name = name;
        _items.AddRange(items ?? new List<ItemEntity>());
        _tags.AddRange(tags ?? new List<TagEntity>());
    }
    
    public static ErrorOr<ShoppingListEntity> Create(UserId userId, string name)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        var entity = new ShoppingListEntity(
            userId: userId,
            name: name.Trim(),
            id: null,
            items: null,
            tags: null);

        return entity;
    }
    
    public static ErrorOr<ShoppingListEntity> Restore(ShoppingListId id, string name, UserId userId, ICollection<ItemEntity> items, ICollection<TagEntity> tags)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        var entity = new ShoppingListEntity(
            userId: userId,
            name: name.Trim(),
            id: id,
            items: items,
            tags: tags);

        return entity;
    }
    
    public ErrorOr<Success> UpdateName(string name)
    {
        var nameValidation = DomainValidators.NameValidator(name, MaxNameLength);
        
        if (nameValidation.IsError) return nameValidation.Errors;

        Name = name.Trim();
        return Result.Success;
    }
    
    public ErrorOr<Success> AddItem(ItemEntity item)
    {
        if (_items.Any(x => x.Id == item.Id))
        {
            return Error.Conflict("Item.AlreadyExists", "The item already exists in the shopping list.");
        }
        
        _items.Add(item);
        return Result.Success;
    }
    
    public void RemoveItem(ItemId itemId)
    {
        var itemToRemove = _items.FirstOrDefault(i => i.Id == itemId);
        if (itemToRemove is not null)
        {
            _items.Remove(itemToRemove);
        }
    }
    
    public ErrorOr<Success> AddTag(TagEntity tag)
    {
        if (_tags.Any(t => t.Id == tag.Id))
        {
            return Error.Conflict("Tag.AlreadyExists", "The tag already exists in the shopping list.");
        }
        
        _tags.Add(tag);
        return Result.Success;
    }
    
    public void RemoveTag(TagId tagId)
    {
        var tagToRemove = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tagToRemove is not null)
        {
            _tags.Remove(tagToRemove);
        }
    }
    
    public ErrorOr<Success> AddItems(List<ItemEntity> items)
    {
        var newItemsSet = new HashSet<ItemId>();
        foreach (var item in items)
        {
            if (_items.Any(existing => existing.Id == item.Id) || !newItemsSet.Add(item.Id))
            {
                return Error.Conflict("Item.AlreadyExists", "The item already exists in the shopping list.");
            }
        }

        _items.AddRange(items);

        return Result.Success;
    }
    
    public ErrorOr<Success> AddTags(List<TagEntity> tags)
    {
        var newTagsSet = new HashSet<TagId>();
        foreach (var tag in tags)
        {
            if (_tags.Any(existing => existing.Id == tag.Id) || !newTagsSet.Add(tag.Id))
            {
                return Error.Conflict("Tag.AlreadyExists", "The tag already exists in the shopping list.");
            }
        }
        
        _tags.AddRange(tags);
        
        return Result.Success;
    }
}