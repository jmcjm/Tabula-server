using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface IItemRepository
{
    Task<ErrorOr<ItemEntity>> GetByIdAsync(ItemId id, CancellationToken cancellationToken = default);
    Task<List<ItemEntity>> GetAllByShoppingListIdAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddAsync(ItemEntity itemEntity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateAsync(ItemEntity itemEntity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(ItemId id, CancellationToken cancellationToken = default);
}