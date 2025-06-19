using Legacy.DataAccess.Enums;

namespace Legacy.DataAccess.DTOs.Responses;

public record ShareResponseModel(
    Guid Id,
    Guid ShoppingListId,
    string ShoppingListName,
    string OwnerUsername,
    string SharedWithUsername,
    SharePermission Permission,
    DateTime SharedAt
); 