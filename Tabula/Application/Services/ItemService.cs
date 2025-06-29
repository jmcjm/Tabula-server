using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Application.Services;

public class ItemService(IItemRepository itemRepository, IShoppingListService shoppingListService) : IItemService
{
    public async Task<ErrorOr<ItemEntity>> AddItemAsync(AddItemCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await shoppingListService.CheckPermissionsAndGetShoppingListAsync(
                command.ShoppingListId, userId, SharePermission.Modify, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var shoppingList = authorizationResult.Value;
        var createResult = ItemEntity.Create(command.ProductName, command.Quantity, command.Bought, shoppingList.Id);
        
        if (createResult.IsError)
            return createResult.Errors;
        
        var addResult = await itemRepository.AddAsync(createResult.Value, cancellationToken);
        return addResult.IsError ? addResult.Errors : createResult.Value;
    }
    
    public async Task<ErrorOr<ItemEntity>> UpdateItemAsync(UpdateItemCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var itemResult = await itemRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (itemResult.IsError)
            return itemResult.Errors;
        
        var item = itemResult.Value;
        
        var authorizationResult = await shoppingListService.CheckPermissionsAndGetShoppingListAsync(
            item.ShoppingListId, userId, SharePermission.Modify, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        item.UpdateBought(command.Bought);
        item.UpdateQuantity(command.Quantity);
        var nameUpdateResult = item.UpdateName(command.ProductName);

        if (nameUpdateResult.IsError)
            return nameUpdateResult.Errors;
        
        var updateResult = await itemRepository.UpdateAsync(item, cancellationToken);
        
        if (updateResult.IsError)
            return updateResult.Errors;
        
        return item;
    }
    
    public async Task<ErrorOr<Deleted>> DeleteItemAsync(DeleteItemCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var itemResult = await itemRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (itemResult.IsError)
            return itemResult.Errors;
        
        var item = itemResult.Value;
        
        var authorizationResult = await shoppingListService.CheckPermissionsAndGetShoppingListAsync(
            item.ShoppingListId, userId, SharePermission.Admin, cancellationToken: cancellationToken);
        
        if (authorizationResult.IsError)
            return authorizationResult.Errors;
        
        var deleteResult = await itemRepository.DeleteAsync(command.Id, cancellationToken);
        
        if (deleteResult.IsError)
            return deleteResult.Errors;
        
        return Result.Deleted;
    }
}