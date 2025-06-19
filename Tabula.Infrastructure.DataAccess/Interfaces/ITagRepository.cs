using ErrorOr;
using Tabula.Infrastructure.DataAccess.Entities;

namespace Tabula.Infrastructure.DataAccess.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllByUserIdAsync(string userId);
    Task<ErrorOr<Tag>> GetByIdAsync(Guid id);
    Task<ErrorOr<Success>> AddAsync(Tag tag);
    Task<ErrorOr<Success>> DeleteAsync(Guid id, bool forceDelete = false);
    Task<List<Tag>> GetTagsByShoppingListIdAsync(Guid shoppingListId);
    Task<bool> IsTagUsedAsync(Guid tagId);
    Task<ErrorOr<Success>> AddToShoppingListAsync(Guid shoppingListId, Guid tagId);
    Task<ErrorOr<Success>> RemoveFromShoppingListAsync(Guid shoppingListId, Guid tagId);
    Task<bool> IsTagAssignedToListAsync(Guid shoppingListId, Guid tagId);
    Task<int> GetTagCountForShoppingListAsync(Guid shoppingListId);
} 