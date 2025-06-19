namespace Infrastructure.DbModels;

public class ShareDbModel
{
    public Guid Id { get; set; }
    public Guid ShoppingListId { get; set; }
    public required string SharedWithUserId { get; set; }
    public int Permission { get; set; }
    public DateTime SharedAt { get; set; }
    public ShoppingListDbModel? ShoppingList { get; set; }
}