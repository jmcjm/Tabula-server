using ErrorOr;
using Tabula.Infrastructure.DataAccess.Entities;

namespace Tabula.Infrastructure.DataAccess.Interfaces;

public interface IItemRepository
{
    Task<ErrorOr<Item>> GetByIdAsync(Guid id);
    Task<List<Item>> GetAllByShoppingListIdAsync(Guid shoppingListId);
    Task<ErrorOr<Success>> AddAsync(Item item);
    Task<ErrorOr<Success>> UpdateAsync(Item item);
    Task<ErrorOr<Success>> DeleteAsync(Guid id);
}