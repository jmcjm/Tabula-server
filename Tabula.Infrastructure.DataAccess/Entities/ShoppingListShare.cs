using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Entities;

public class ShoppingListShare : BaseEntity
{
    public Guid ShoppingListId { get; set; } // ID listy zakupów
    public string OwnerId { get; set; } = string.Empty; // ID właściciela (IdentityUser.Id)
    public string SharedWithUserId { get; set; } = string.Empty; // ID użytkownika z którym udostępniono
    public SharePermission Permission { get; set; } // Typ uprawnień
    public DateTime SharedAt { get; set; } = DateTime.UtcNow; // Kiedy udostępniono
} 