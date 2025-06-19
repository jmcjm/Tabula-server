using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Models;

public class ShoppingListShareDao
{
    public Guid Id { get; set; }
    public Guid ShoppingListId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string SharedWithUserId { get; set; } = string.Empty;
    public SharePermission Permission { get; set; }
    public DateTime SharedAt { get; set; }
    
    // Relacje nawigacyjne
    public ShoppingListDao ShoppingList { get; set; } = null!;
} 