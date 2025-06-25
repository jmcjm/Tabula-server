using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class ShareRepository : IShareRepository
{
    public Task<ErrorOr<ShareEntity>> GetByIdAsync(ShareId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShareEntity>> GetSharedWithUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShareEntity>> GetSharedByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShareEntity>> GetSharesForShoppingListAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<ShareEntity>> GetShareAsync(ShoppingListId shoppingListId, UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddAsync(ShareEntity shareEntity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(ShareEntity shareEntity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(ShareId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}