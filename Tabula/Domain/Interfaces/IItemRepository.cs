using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface IItemRepository
{
    Task<ErrorOr<ItemEntity>> GetByIdAsync(ItemId id);
    Task<List<ItemEntity>> GetAllByShoppingListIdAsync(ShoppingListId shoppingListId);
    Task<ErrorOr<Success>> AddAsync(ItemEntity itemEntity);
    Task<ErrorOr<Success>> UpdateAsync(ItemEntity itemEntity);
    Task<ErrorOr<Success>> DeleteAsync(ItemId id);
}