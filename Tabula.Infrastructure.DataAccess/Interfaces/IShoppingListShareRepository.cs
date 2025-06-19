using ErrorOr;
using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Enums;

namespace Tabula.Infrastructure.DataAccess.Interfaces;

public interface IShoppingListShareRepository
{
    Task<ErrorOr<ShoppingListShare>> GetByIdAsync(Guid id);
    Task<List<ShoppingListShare>> GetSharedWithUserAsync(string userId);
    Task<List<ShoppingListShare>> GetSharedByUserAsync(string userId);
    Task<List<ShoppingListShare>> GetSharesForShoppingListAsync(Guid shoppingListId);
    Task<ShoppingListShare?> GetShareAsync(Guid shoppingListId, string userId);
    Task<ErrorOr<Success>> AddAsync(ShoppingListShare share);
    Task<ErrorOr<Success>> UpdateAsync(ShoppingListShare share);
    Task<ErrorOr<Success>> DeleteAsync(Guid id);
    Task<ErrorOr<Success>> DeleteByShoppingListAndUserAsync(Guid shoppingListId, string userId);
    Task<ErrorOr<ShoppingListShare>> GetShareIfAuthorizedAsync(string userId, Guid shoppingListId, SharePermission requiredPermission);
    Task<bool> HasPermissionAsync(string userId, Guid shoppingListId, SharePermission requiredPermission);
} 