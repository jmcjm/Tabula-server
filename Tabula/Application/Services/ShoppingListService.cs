using Application.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Application.Services;

public class ShoppingListService(IShoppingListRepository shoppingListRepository, IShareRepository shareRepository)
{
    public async Task<ErrorOr<ShoppingListEntity>> CreateShoppingListAsync(CreateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        // We don't care about possible duplicates here (same UserId and Name) as we're using unique index on db so we will catch them on INSERT
        var creationResult = ShoppingListEntity.Create(userId, command.Name);
        if (creationResult.IsError)
            return creationResult.Errors;
        
        var shoppingListEntity = creationResult.Value;

        var addResult = await shoppingListRepository.AddAsync(shoppingListEntity, cancellationToken);
        return addResult.IsError ? addResult.Errors : shoppingListEntity;
    }
    
    public async Task<ErrorOr<ShoppingListEntity>> UpdateShoppingListAsync(UpdateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(
            command.Id, userId, SharePermission.Modify, cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;

        var nameUpdateResult = shoppingList.UpdateName(command.Name);
        if (nameUpdateResult.IsError)
            return nameUpdateResult.Errors;

        var updateResult = await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        return updateResult.IsError ? updateResult.Errors : shoppingList;
    }
    
    public async Task<ErrorOr<Deleted>> DeleteShoppingListAsync(DeleteShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(
            command.Id, userId, SharePermission.Admin, cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;

        var deleteResult = await shoppingListRepository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
        return deleteResult.IsError ? deleteResult.Errors : Result.Deleted;
    }
    
    public async Task<ErrorOr<ShoppingListEntity>> CheckPermissionsAndGetShoppingListAsync(
        ShoppingListId shoppingListId, 
        UserId userId, 
        SharePermission requiredPermission,
        CancellationToken cancellationToken = default)
    {
        var shoppingListResult = await shoppingListRepository.GetByIdAsync(shoppingListId, cancellationToken);
        if (shoppingListResult.IsError)
            return shoppingListResult.Errors;

        var shoppingList = shoppingListResult.Value;

        // Owner has all permissions
        if (shoppingList.UserId == userId)
            return shoppingList;

        // Check share permissions
        var shareResults = await shareRepository.GetShareAsync(shoppingListId, userId, cancellationToken);
        
        if (shareResults.IsError)
            return shareResults.Errors;

        var share = shareResults.Value;
        
        // Check if user has required permission level
        if (!HasRequiredPermission(share.Permission, requiredPermission))
            return Error.Forbidden(
                code: "ShoppingList.Forbidden", 
                description: GetPermissionErrorMessage(requiredPermission));

        return shoppingList;
    }

    private static bool HasRequiredPermission(SharePermission userPermission, SharePermission requiredPermission)
    {
        // Admin can do everything, Modify can read and write, Read can only read
        return requiredPermission switch
        {
            SharePermission.Read => userPermission >= SharePermission.Read,
            SharePermission.Modify => userPermission >= SharePermission.Modify,
            SharePermission.Admin => userPermission == SharePermission.Admin,
            _ => false
        };
    }

    private static string GetPermissionErrorMessage(SharePermission requiredPermission)
    {
        return requiredPermission switch
        {
            SharePermission.Read => "You do not have permission to view this shopping list.",
            SharePermission.Modify => "You do not have permission to modify this shopping list.",
            SharePermission.Admin => "You do not have permission to perform administrative actions on this shopping list.",
            _ => "You do not have sufficient permissions for this operation."
        };
    }
}