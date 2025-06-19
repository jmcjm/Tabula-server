using Legacy.DataAccess.Enums;

namespace Legacy.DataAccess.DTOs.Requests;

public sealed record ShareRequestModel(
    Guid ShoppingListId,
    string SharedWithUserId,
    SharePermission Permission
); 