using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class ShoppingListRepository() : IShoppingListRepository
{
    public Task<ErrorOr<ShoppingListEntity>> GetByIdAsync(ShoppingListId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShoppingListEntity>> GetAllByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<ShoppingListEntity>> GetByIdAndUserIdAsync(ShoppingListId id, UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<ShoppingListEntity>> AddAsync(ShoppingListEntity shoppingList, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(ShoppingListEntity shoppingList, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(ShoppingListId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}