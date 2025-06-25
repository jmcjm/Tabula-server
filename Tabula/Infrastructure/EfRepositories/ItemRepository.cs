using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class ItemRepository : IItemRepository
{
    public Task<ErrorOr<ItemEntity>> GetByIdAsync(ItemId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ItemEntity>> GetAllByShoppingListIdAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddAsync(ItemEntity itemEntity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(ItemEntity itemEntity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(ItemId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}