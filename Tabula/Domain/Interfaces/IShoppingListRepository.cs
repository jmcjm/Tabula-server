using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface IShoppingListRepository
{
    Task<ErrorOr<ShoppingListEntity>> GetByIdAsync(ShoppingListId id, CancellationToken cancellationToken = default);
    Task<List<ShoppingListEntity>> GetAllByUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<ShoppingListEntity>> GetByIdAndUserIdAsync(ShoppingListId id, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddAsync(ShoppingListEntity shoppingList, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateAsync(ShoppingListEntity shoppingList, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(ShoppingListId id, CancellationToken cancellationToken = default);
}