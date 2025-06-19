using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class ItemRepository : IItemRepository
{
    public Task<ErrorOr<ItemEntity>> GetByIdAsync(ItemId id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ItemEntity>> GetAllByShoppingListIdAsync(ShoppingListId shoppingListId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddAsync(ItemEntity itemEntity)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(ItemEntity itemEntity)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(ItemId id)
    {
        throw new NotImplementedException();
    }
}