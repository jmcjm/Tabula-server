using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface IShoppingListRepository
{
    Task<ErrorOr<ShoppingListEntity>> GetByIdAsync(ShoppingListId id);
    Task<List<ShoppingListEntity>> GetAllByUserAsync(UserId userId);
    Task<ErrorOr<Success>> AddAsync(ShoppingListEntity shoppingList);
    Task<ErrorOr<Success>> UpdateAsync(ShoppingListEntity shoppingList);
    Task<ErrorOr<Success>> DeleteAsync(ShoppingListId id);
}