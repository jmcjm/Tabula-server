using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface ITagRepository
{
    Task<List<TagEntity>> GetAllByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<TagEntity>> GetByIdAsync(TagId id, CancellationToken cancellationToken = default);
    Task<ErrorOr<TagEntity>> GetByIdAndUserIdAsync(TagId id, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddAsync(TagEntity tagEntity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(TagId id, bool forceDelete = false, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateAsync(TagEntity tagEntity, CancellationToken cancellationToken = default);
    Task<List<TagEntity>> GetTagsByShoppingListIdAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default);
    Task<bool> IsTagUsedAsync(TagId tagId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddToShoppingListAsync(ShoppingListId shoppingListId, TagId tagId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> RemoveFromShoppingListAsync(ShoppingListId shoppingListId, TagId tagId, CancellationToken cancellationToken = default);
    Task<int> GetTagCountForShoppingListAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default);
} 