using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Models;

public record ShareRequestModel(
    Guid ShoppingListId,
    string SharedWithUsername,
    SharePermission Permission
); 