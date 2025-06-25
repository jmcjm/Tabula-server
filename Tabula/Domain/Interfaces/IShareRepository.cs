using Domain.Entities;
using Domain.Enums;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface IShareRepository
{
    Task<ErrorOr<ShareEntity>> GetByIdAsync(ShareId id, CancellationToken cancellationToken = default);
    Task<List<ShareEntity>> GetSharedWithUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<List<ShareEntity>> GetSharedByUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<List<ShareEntity>> GetSharesForShoppingListAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default);
    
    // TODO: Rename
    Task<ErrorOr<ShareEntity>> GetShareAsync(ShoppingListId shoppingListId, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddAsync(ShareEntity shareEntity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateAsync(ShareEntity shareEntity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(ShareId id, CancellationToken cancellationToken = default);
} 