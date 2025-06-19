using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Models;

public record ShareResponseModel(
    Guid Id,
    Guid ShoppingListId,
    string ShoppingListName,
    string OwnerUsername,
    string SharedWithUsername,
    SharePermission Permission,
    DateTime SharedAt
); 