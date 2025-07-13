using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Application.Services;

public class ShoppingListService(IShoppingListRepository shoppingListRepository, IShareRepository shareRepository, IItemRepository itemRepository, ITagRepository tagRepository) : IShoppingListService
{
    public async Task<ErrorOr<ShoppingListEntity>> GetShoppingListByIdAsync(ShoppingListId shoppingListId, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(shoppingListId, userId, SharePermission.Read, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        return authorizationResult.Value;
    }
    
    public async Task<ErrorOr<IReadOnlyList<ShoppingListEntity>>> GetShoppingListsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await shoppingListRepository.GetAllByUserAsync(userId, cancellationToken);
    }

    public async Task<ErrorOr<ShoppingListEntity>> CreateShoppingListAsync(CreateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var creationResult = ShoppingListEntity.Create(userId, command.Name);
        if (creationResult.IsError)
            return creationResult.Errors;
        
        var shoppingListEntity = creationResult.Value;

        var addResult = await shoppingListRepository.AddAsync(shoppingListEntity, cancellationToken);
        
        if (addResult.IsError)
            return addResult.Errors;
        
        return addResult.Value;
    }
    
    public async Task<ErrorOr<Updated>> UpdateShoppingListAsync(UpdateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(command.Id, userId, SharePermission.Modify, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;

        var nameUpdateResult = shoppingList.UpdateName(command.Name);
        if (nameUpdateResult.IsError)
            return nameUpdateResult.Errors;

        var updateResult = await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        return updateResult.IsError ? updateResult.Errors : Result.Updated;
    }
    
    public async Task<ErrorOr<Deleted>> DeleteShoppingListAsync(DeleteShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(command.Id, userId, SharePermission.Admin, cancellationToken:cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;

        var deleteResult = await shoppingListRepository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
        return deleteResult.IsError ? deleteResult.Errors : Result.Deleted;
    }
    
    public async Task<ErrorOr<Success>> AddItemToShoppingListAsync(AddItemCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(
            command.ShoppingListId, userId, SharePermission.Modify, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;
        
        var createResult = ItemEntity.Create(command.ProductName, command.Quantity, command.Bought, shoppingList.Id);
        
        if (createResult.IsError)
            return createResult.Errors;
        
        var item = createResult.Value;
        
        var addResult = shoppingList.AddItem(item);
        
        if (addResult.IsError)
            return addResult.Errors;
        
        await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> UpdateItemInShoppingListAsync(UpdateItemCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var itemResult = await itemRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (itemResult.IsError)
            return itemResult.Errors;
        
        var item = itemResult.Value;
        
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(
            item.ShoppingListId, userId, SharePermission.Modify, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;
        
        var itemToUpdate = shoppingList.Items.FirstOrDefault(i => i.Id == command.Id);
        
        if (itemToUpdate is null)
            return Error.NotFound("Item.NotFound", "The item to update was not found in the shopping list.");
            
        itemToUpdate.UpdateBought(command.Bought);
        itemToUpdate.UpdateQuantity(command.Quantity);
        var nameUpdateResult = itemToUpdate.UpdateName(command.ProductName);

        if (nameUpdateResult.IsError)
            return nameUpdateResult.Errors;
        
        await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteItemFromShoppingListAsync(DeleteItemCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var itemResult = await itemRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (itemResult.IsError)
            return itemResult.Errors;
        
        var item = itemResult.Value;
        
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(
            item.ShoppingListId, userId, SharePermission.Admin, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;
        
        shoppingList.RemoveItem(command.Id);
        
        await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> AssignTagToShoppingListAsync(AssignTagCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(command.ShoppingListId, userId, SharePermission.Admin, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;
        
        var tagResult = await tagRepository.GetByIdAsync(command.TagId, cancellationToken);
        
        if (tagResult.IsError)
            return tagResult.Errors;
        
        var tag = tagResult.Value;
        
        if (tag.UserId != userId)
            return Error.Forbidden(
                code: "ShoppingList.Forbidden", 
                description: "You do not have permissions to perform those actions on this shopping list.");
        
        var addResult = shoppingList.AddTag(tag);
        
        if (addResult.IsError)
            return addResult.Errors;
        
        await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        
        return Result.Success;
    }
    
    public async Task<ErrorOr<Success>> RemoveTagFromShoppingListAsync(RemoveTagCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await CheckPermissionsAndGetShoppingListAsync(command.ShoppingListId, userId, SharePermission.Admin, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;
        
        var tagResult = await tagRepository.GetByIdAsync(command.TagId, cancellationToken);
        
        if (tagResult.IsError)
            return tagResult.Errors;
        
        var tag = tagResult.Value;
        
        if (tag.UserId != userId)
            return Error.Forbidden(
                code: "ShoppingList.Forbidden", 
                description: "You do not have permissions to perform those actions on this shopping list.");
        
        shoppingList.RemoveTag(command.TagId);
        
        await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        
        return Result.Success;
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

        if (shoppingList.UserId == userId)
            return shoppingList;

        var shareResults = await shareRepository.GetShareAsync(shoppingListId, userId, cancellationToken);
        
        if (shareResults.IsError)
            return Error.Forbidden(
                code: "ShoppingList.Forbidden", 
                description: "You do not have permissions to perform those actions on this shopping list.");

        var share = shareResults.Value;
        
        if (!HasRequiredPermission(share.Permission, requiredPermission))
            return Error.Forbidden(
                code: "ShoppingList.Forbidden", 
                description: GetPermissionErrorMessage(requiredPermission));

        return shoppingList;
    }

    private static bool HasRequiredPermission(SharePermission userPermission, SharePermission requiredPermission)
    {
        return userPermission >= requiredPermission;
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