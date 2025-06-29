using Application.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.Records;
using ErrorOr;

namespace Application.Interfaces;

public interface IShoppingListService
{
    Task<ErrorOr<ShoppingListEntity>> CreateShoppingListAsync(CreateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<ShoppingListEntity>> UpdateShoppingListAsync(UpdateShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteShoppingListAsync(DeleteShoppingListCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<ShoppingListEntity>> CheckPermissionsAndGetShoppingListAsync(ShoppingListId shoppingListId, UserId userId, SharePermission requiredPermission, bool checkOwner = true, CancellationToken cancellationToken = default);
}