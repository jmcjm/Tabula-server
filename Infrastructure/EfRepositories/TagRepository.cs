using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Infrastructure.EfRepositories;

public class TagRepository : ITagRepository
{
    public Task<List<TagEntity>> GetAllByUserIdAsync(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<TagEntity>> GetByIdAsync(TagId id)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddAsync(TagEntity tagEntity)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(TagId id, bool forceDelete = false)
    {
        throw new NotImplementedException();
    }

    public Task<List<TagEntity>> GetTagsByShoppingListIdAsync(ShoppingListId shoppingListId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTagUsedAsync(TagId tagId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddToShoppingListAsync(ShoppingListId shoppingListId, TagId tagId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> RemoveFromShoppingListAsync(ShoppingListId shoppingListId, TagId tagId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTagCountForShoppingListAsync(ShoppingListId shoppingListId)
    {
        throw new NotImplementedException();
    }
}