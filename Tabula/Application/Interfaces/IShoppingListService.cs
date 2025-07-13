using Application.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.Records;
using ErrorOr;

namespace Application.Interfaces;

public interface IShoppingListService
{
    Task<ErrorOr<ShoppingListEntity>> GetShoppingListByIdAsync(ShoppingListId shoppingListId, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<IReadOnlyList<ShoppingListEntity>>> GetShoppingListsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<ShoppingListEntity>> CreateShoppingListAsync(CreateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateShoppingListAsync(UpdateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteShoppingListAsync(DeleteShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<ShoppingListEntity>> CheckPermissionsAndGetShoppingListAsync(ShoppingListId shoppingListId, UserId userId, SharePermission permission, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddItemToShoppingListAsync(AddItemCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateItemInShoppingListAsync(UpdateItemCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteItemFromShoppingListAsync(DeleteItemCommand command, UserId userId, CancellationToken cancellationToken = default);
}