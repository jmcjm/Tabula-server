using ErrorOr;
using Tabula.Infrastructure.DataAccess.Entities;

namespace Tabula.Infrastructure.DataAccess.Interfaces;

public interface IShoppingListRepository
{
    Task<ErrorOr<ShoppingListEntity>> GetByIdAsync(Guid id);
    Task<List<ShoppingListEntity>> GetAllByUserAsync(string userId);
    Task<ErrorOr<Success>> AddAsync(ShoppingListEntity shoppingList);
    Task<ErrorOr<Success>> UpdateAsync(ShoppingListEntity shoppingList);
    Task<ErrorOr<Success>> DeleteAsync(Guid id);
}