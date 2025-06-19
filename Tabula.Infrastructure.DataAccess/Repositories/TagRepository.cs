using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Tabula.Infrastructure.DataAccess.Mappers;
using Tabula.Infrastructure.DataAccess.Database;
using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Errors;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Infrastructure.DataAccess.Repositories;

public class TagRepository(ShoppingListDbContext context) : ITagRepository
{
    public async Task<List<Tag>> GetAllByUserIdAsync(string userId)
    {
        var daos = await context.Tags
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Name)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<ErrorOr<Tag>> GetByIdAsync(Guid id)
    {
        var dao = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);

        if (dao == null)
            return TagErrors.NotFound(id);

        return dao.ToDomain();
    }

    public async Task<ErrorOr<Success>> AddAsync(Tag tag)
    {
        try
        {
            var dao = tag.ToDao();
            await context.Tags.AddAsync(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
                return TagErrors.AlreadyExists(tag.Name);
            
            return Error.Failure("Tag.AddFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id, bool forceDelete = false)
    {
        var dao = await context.Tags.FindAsync(id);

        if (dao == null)
            return TagErrors.NotFound(id);

        var isTagUsed = await IsTagUsedAsync(id);
        
        if (isTagUsed && !forceDelete)
            return TagErrors.InUse(id);

        try
        {
            if (isTagUsed && forceDelete)
            {
                // Usuń wszystkie powiązania z listami zakupów
                var shoppingListsWithTag = await context.ShoppingLists
                    .Include(sl => sl.Tags)
                    .Where(sl => sl.Tags.Any(t => t.Id == id))
                    .ToListAsync();

                foreach (var shoppingList in shoppingListsWithTag)
                {
                    var tagToRemove = shoppingList.Tags.First(t => t.Id == id);
                    shoppingList.Tags.Remove(tagToRemove);
                }
            }

            context.Tags.Remove(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Tag.DeleteFailed", ex.Message);
        }
    }

    public async Task<List<Tag>> GetTagsByShoppingListIdAsync(Guid shoppingListId)
    {
        var daos = await context.Tags
            .Where(t => t.ShoppingLists.Any(sl => sl.Id == shoppingListId))
            .OrderBy(t => t.Name)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<bool> IsTagUsedAsync(Guid tagId)
    {
        return await context.Tags
            .Where(t => t.Id == tagId)
            .SelectMany(t => t.ShoppingLists)
            .AnyAsync();
    }

    public async Task<ErrorOr<Success>> AddToShoppingListAsync(Guid shoppingListId, Guid tagId)
    {
        var shoppingListDao = await context.ShoppingLists
            .Include(sl => sl.Tags)
            .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

        if (shoppingListDao == null)
            return Error.NotFound("ShoppingList.NotFound", $"ShoppingList with ID {shoppingListId} not found.");

        var tagDao = await context.Tags.FindAsync(tagId);

        if (tagDao == null)
            return TagErrors.NotFound(tagId);

        if (await GetTagCountForShoppingListAsync(shoppingListId) >= 5)
            return TagErrors.LimitExceeded();

        if (shoppingListDao.Tags.Any(t => t.Id == tagId)) return Result.Success;
        
        shoppingListDao.Tags.Add(tagDao);
        await context.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> RemoveFromShoppingListAsync(Guid shoppingListId, Guid tagId)
    {
        var shoppingListDao = await context.ShoppingLists
            .Include(sl => sl.Tags)
            .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

        if (shoppingListDao == null)
            return Error.NotFound("ShoppingList.NotFound", $"ShoppingList with ID {shoppingListId} not found.");

        var tagToRemove = shoppingListDao.Tags.FirstOrDefault(t => t.Id == tagId);
        
        if (tagToRemove == null) return Result.Success;
        
        shoppingListDao.Tags.Remove(tagToRemove);
        await context.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<bool> IsTagAssignedToListAsync(Guid shoppingListId, Guid tagId)
    {
        return await context.ShoppingLists
            .Where(sl => sl.Id == shoppingListId)
            .SelectMany(sl => sl.Tags)
            .AnyAsync(t => t.Id == tagId);
    }

    public async Task<int> GetTagCountForShoppingListAsync(Guid shoppingListId)
    {
        return await context.ShoppingLists
            .Where(sl => sl.Id == shoppingListId)
            .SelectMany(sl => sl.Tags)
            .CountAsync();
    }
} 