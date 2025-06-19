using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Domain.Interfaces;

public interface ITagRepository
{
    Task<List<TagEntity>> GetAllByUserIdAsync(UserId userId);
    Task<ErrorOr<TagEntity>> GetByIdAsync(TagId id);
    Task<ErrorOr<Success>> AddAsync(TagEntity tagEntity);
    Task<ErrorOr<Success>> DeleteAsync(TagId id, bool forceDelete = false);
    Task<List<TagEntity>> GetTagsByShoppingListIdAsync(ShoppingListId shoppingListId);
    Task<bool> IsTagUsedAsync(TagId tagId);
    Task<ErrorOr<Success>> AddToShoppingListAsync(ShoppingListId shoppingListId, TagId tagId);
    Task<ErrorOr<Success>> RemoveFromShoppingListAsync(ShoppingListId shoppingListId, TagId tagId);
    Task<int> GetTagCountForShoppingListAsync(ShoppingListId shoppingListId);
} 