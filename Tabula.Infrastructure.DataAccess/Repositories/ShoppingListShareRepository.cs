using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Tabula.Infrastructure.DataAccess.Mappers;
using Tabula.Infrastructure.DataAccess.Database;
using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Enums;
using Tabula.Infrastructure.DataAccess.Errors;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Infrastructure.DataAccess.Repositories;

public class ShoppingListShareRepository(ShoppingListDbContext context) : IShoppingListShareRepository
{
    public async Task<ErrorOr<ShoppingListShare>> GetByIdAsync(Guid id)
    {
        var dao = await context.ShoppingListShares
            .Include(s => s.ShoppingList)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (dao == null)
            return ShareErrors.NotFound(id);

        return dao.ToDomain();
    }

    public async Task<List<ShoppingListShare>> GetSharedWithUserAsync(string userId)
    {
        var daos = await context.ShoppingListShares
            .Include(s => s.ShoppingList)
            .Where(s => s.SharedWithUserId == userId)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<List<ShoppingListShare>> GetSharedByUserAsync(string userId)
    {
        var daos = await context.ShoppingListShares
            .Include(s => s.ShoppingList)
            .Where(s => s.OwnerId == userId)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<List<ShoppingListShare>> GetSharesForShoppingListAsync(Guid shoppingListId)
    {
        var daos = await context.ShoppingListShares
            .Where(s => s.ShoppingListId == shoppingListId)
            .ToListAsync();

        return daos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<ShoppingListShare?> GetShareAsync(Guid shoppingListId, string userId)
    {
        var dao = await context.ShoppingListShares
            .FirstOrDefaultAsync(s => s.ShoppingListId == shoppingListId && s.SharedWithUserId == userId);

        return dao?.ToDomain();
    }

    public async Task<ErrorOr<Success>> AddAsync(ShoppingListShare share)
    {
        try
        {
            var dao = share.ToDao();
            await context.ShoppingListShares.AddAsync(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
                return ShareErrors.AlreadyShared(share.SharedWithUserId);
            
            return Error.Failure("Share.AddFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateAsync(ShoppingListShare share)
    {
        var dao = await context.ShoppingListShares.FindAsync(share.Id);

        if (dao == null)
            return ShareErrors.NotFound(share.Id);

        try
        {
            dao.Permission = share.Permission;
            dao.SharedAt = share.SharedAt;

            context.ShoppingListShares.Update(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Share.UpdateFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id)
    {
        var dao = await context.ShoppingListShares.FindAsync(id);

        if (dao == null)
            return ShareErrors.NotFound(id);

        try
        {
            context.ShoppingListShares.Remove(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Share.DeleteFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeleteByShoppingListAndUserAsync(Guid shoppingListId, string userId)
    {
        var dao = await context.ShoppingListShares
            .FirstOrDefaultAsync(s => s.ShoppingListId == shoppingListId && s.SharedWithUserId == userId);

        if (dao == null)
            return ShareErrors.NotFound(shoppingListId);

        try
        {
            context.ShoppingListShares.Remove(dao);
            await context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Share.DeleteFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<ShoppingListShare>> GetShareIfAuthorizedAsync(string userId, Guid shoppingListId, SharePermission requiredPermission)
    {
        var dao = await context.ShoppingListShares
            .FirstOrDefaultAsync(s => s.ShoppingListId == shoppingListId && s.SharedWithUserId == userId);

        if (dao == null || dao.Permission < requiredPermission)
            return Error.Forbidden("Share.AccessDenied", "You don't have permission to perform this action.");

        return dao.ToDomain();
    }

    public async Task<bool> HasPermissionAsync(string userId, Guid shoppingListId, SharePermission requiredPermission)
    {
        var dao = await context.ShoppingListShares
            .FirstOrDefaultAsync(s => s.ShoppingListId == shoppingListId && s.SharedWithUserId == userId);

        return dao != null && dao.Permission >= requiredPermission;
    }
} 