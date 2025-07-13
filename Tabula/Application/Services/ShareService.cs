using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Application.Services;

public class ShareService(IShareRepository shareRepository, IShoppingListService shoppingListService)
{
    public async Task<ErrorOr<Success>> ShareAsync(CreateShareCommand command, UserId userId)
    {
        var shpList = await shoppingListService.CheckPermissionsAndGetShoppingListAsync(command.ShoppingListId, userId, SharePermission.Modify);
        
        if (shpList.IsError)
            return shpList.Errors;
        
        var shareEntityResult = ShareEntity.Create(
            shoppingListId: command.ShoppingListId,
            sharedWithUserId: command.SharedWithUserId,
            permission: command.Permission
        );
        
        if (shareEntityResult.IsError)
            return shareEntityResult.Errors;
        
        var createShareResult = await shareRepository.AddAsync(shareEntityResult.Value);
        
        if (createShareResult.IsError)
            return createShareResult.Errors;
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteShareAsync(DeleteShareCommand command, UserId userId)
    {
        var shareResult = await shareRepository.GetByIdAsync(command.Id);
        
        if (shareResult.IsError)
            return shareResult.Errors;
        
        var shpList = await shoppingListService.CheckPermissionsAndGetShoppingListAsync(shareResult.Value.ShoppingListId, userId, SharePermission.Modify);
        
        if (shpList.IsError)
            return shpList.Errors;
        
        var deleteResult = await shareRepository.DeleteAsync(command.Id);
        
        if (deleteResult.IsError)
            return deleteResult.Errors;
        
        return Result.Success;
    }
}