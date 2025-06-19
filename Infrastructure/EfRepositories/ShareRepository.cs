using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class ShareRepository : IShareRepository
{
    public Task<ErrorOr<ShareEntity>> GetByIdAsync(ShareId id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShareEntity>> GetSharedWithUserAsync(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShareEntity>> GetSharedByUserAsync(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShareEntity>> GetSharesForShoppingListAsync(ShoppingListId shoppingListId)
    {
        throw new NotImplementedException();
    }

    public Task<ShareEntity?> GetShareAsync(ShoppingListId shoppingListId, UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddAsync(ShareEntity shareEntity)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(ShareEntity shareEntity)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(ShareId id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasPermissionAsync(UserId userId, ShoppingListId shoppingListId, SharePermission requiredPermission)
    {
        throw new NotImplementedException();
    }
}