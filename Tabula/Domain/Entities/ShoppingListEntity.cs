using Domain.Records;

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
    
    public ShoppingListEntity(UserId userId, string name, ShoppingListId? id)
    {
        Id = id ?? new ShoppingListId(Guid.NewGuid());
        UserId = userId;
        Name = name;
    }
    
    private void AddItems(List<ItemEntity> items)
    {
        _items.AddRange(items);
    }
    
    private void AddTags(List<TagEntity> tags)
    {
        _tags.AddRange(tags);
    }
}