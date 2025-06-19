namespace Tabula.Infrastructure.DataAccess.Models;

public class ShoppingListDao
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public List<ItemDao> Items { get; private set; }
    public string UserId { get; private set; }
    public List<TagDao> Tags { get; private set; }
    
    public ShoppingListDao(Guid id, string userId, string name, List<ItemDao> items, List<TagDao> tags)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Items = items;
        Tags = tags;
    }
    
    public void EditItems(List<ItemDao> items)
    {
        Items = items;
    }

    public void EditTags(List<TagDao> tags)
    {
        Tags = tags;
    }
    
    public void ChangeName(string name)
    {
        Name = name;
    }
}