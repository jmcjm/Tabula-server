using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class ShoppingListRepository : IShoppingListRepository
{
    public Task<ErrorOr<ShoppingListEntity>> GetByIdAsync(ShoppingListId id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShoppingListEntity>> GetAllByUserAsync(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddAsync(ShoppingListEntity shoppingList)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(ShoppingListEntity shoppingList)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(ShoppingListId id)
    {
        throw new NotImplementedException();
    }
}