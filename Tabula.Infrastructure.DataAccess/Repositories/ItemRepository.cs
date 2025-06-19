using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using Tabula.Infrastructure.DataAccess.Mappers;
using ErrorOr;
using Tabula.Infrastructure.DataAccess.Database;
using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Errors;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Infrastructure.DataAccess.Repositories;

public class ItemRepository(ShoppingListDbContext context) : IItemRepository
{
    public async Task<ErrorOr<Item>> GetByIdAsync(Guid id)
    {
        var dao = await context.Items.FindAsync(id);

        if (dao == null)
            return ItemErrors.NotFound(id);

        return dao.ToDomain();
    }

    public async Task<List<Item>> GetAllByShoppingListIdAsync(Guid shoppingListId)
    {
        var daos = await context.Items
            .Where(i => i.ShoppingListId == shoppingListId)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<ErrorOr<Success>> AddAsync(Item item)
    {
        try
        {
            var dao = item.ToDao();
            await context.Items.AddAsync(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Item.AddFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateAsync(Item item)
    {
        var dao = await context.Items.FindAsync(item.Id);

        if (dao == null)
            return ItemErrors.NotFound(item.Id);

        dao.ProductName = item.ProductName;
        dao.Quantity = item.Quantity;
        context.Items.Update(dao);
        await context.SaveChangesAsync();
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id)
    {
        var dao = await context.Items.FindAsync(id);

        if (dao == null)
            return ItemErrors.NotFound(id);

        context.Items.Remove(dao);
        await context.SaveChangesAsync();
        return Result.Success;
    }
}