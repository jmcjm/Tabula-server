using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Tabula.Infrastructure.DataAccess.Mappers;
using Tabula.Infrastructure.DataAccess.Database;
using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Errors;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Infrastructure.DataAccess.Repositories;

public class ShoppingListRepository(ShoppingListDbContext context) : IShoppingListRepository
{
    public async Task<ErrorOr<ShoppingListEntity>> GetByIdAsync(Guid id)
    {
        var dao = await context.ShoppingLists
            .Include(sl => sl.Tags)
            .FirstOrDefaultAsync(sl => sl.Id == id);

        if (dao == null)
            return ShoppingListErrors.NotFound(id);

        return dao.ToDomain();
    }

    public async Task<List<ShoppingListEntity>> GetAllByUserAsync(string userId)
    {
        var daos = await context.ShoppingLists
            .Include(sl => sl.Tags)
            .Where(sl => sl.UserId == userId)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<ErrorOr<Success>> AddAsync(ShoppingListEntity shoppingList)
    {
        try
        {
            var dao = shoppingList.ToDao();
            await context.ShoppingLists.AddAsync(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
                return ShoppingListErrors.AlreadyExists(shoppingList.Name);
            
            return Error.Failure("ShoppingList.AddFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateAsync(ShoppingListEntity shoppingList)
    {
        var dao = await context.ShoppingLists.FindAsync(shoppingList.Id);

        if (dao == null)
            return ShoppingListErrors.NotFound(shoppingList.Id);

        try
        {
            dao.Name = shoppingList.Name;
            context.ShoppingLists.Update(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
                return ShoppingListErrors.AlreadyExists(shoppingList.Name);
            
            return Error.Failure("ShoppingList.UpdateFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id)
    {
        var dao = await context.ShoppingLists.FindAsync(id);

        if (dao == null)
            return ShoppingListErrors.NotFound(id);

        try
        {
            context.ShoppingLists.Remove(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("ShoppingList.DeleteFailed", ex.Message);
        }
    }
}
