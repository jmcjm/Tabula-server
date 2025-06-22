using Application.Commands;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;

namespace Application.Services;

public class ShoppingListService(IShoppingListRepository shoppingListRepository)
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
        // We query the DB by Id and UserId (which we supply from JWT),
        // so the user cannot access or update shopping lists they do not own.
        var shoppingListResult = await shoppingListRepository.GetByIdAndUserIdAsync(command.Id, userId, cancellationToken);
        if (shoppingListResult.IsError)
            return shoppingListResult.Errors;

        var shoppingList = shoppingListResult.Value;

        var nameUpdateResult = shoppingList.UpdateName(command.Name);
        if (nameUpdateResult.IsError)
            return nameUpdateResult.Errors;

        var updateResult = await shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        return updateResult.IsError ? updateResult.Errors : shoppingList;
    }
    
    public async Task<ErrorOr<Deleted>> DeleteShoppingListAsync(DeleteShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var shoppingListResult = await shoppingListRepository.GetByIdAndUserIdAsync(command.Id, userId, cancellationToken);
        if (shoppingListResult.IsError)
            return shoppingListResult.Errors;

        var deleteResult = await shoppingListRepository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
        return deleteResult.IsError ? deleteResult.Errors : Result.Deleted;
    }
}