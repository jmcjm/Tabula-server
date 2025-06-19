using Domain.Entities;
using Domain.Enums;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface IShareRepository
{
    Task<ErrorOr<ShareEntity>> GetByIdAsync(ShareId id);
    Task<List<ShareEntity>> GetSharedWithUserAsync(UserId userId);
    Task<List<ShareEntity>> GetSharedByUserAsync(UserId userId);
    Task<List<ShareEntity>> GetSharesForShoppingListAsync(ShoppingListId shoppingListId);
    Task<ShareEntity?> GetShareAsync(ShoppingListId shoppingListId, UserId userId);
    Task<ErrorOr<Success>> AddAsync(ShareEntity shareEntity);
    Task<ErrorOr<Success>> UpdateAsync(ShareEntity shareEntity);
    Task<ErrorOr<Success>> DeleteAsync(ShareId id);
    Task<bool> HasPermissionAsync(UserId userId, ShoppingListId shoppingListId, SharePermission requiredPermission);
} 