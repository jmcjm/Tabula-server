using Domain.Enums;
using Domain.Records;

namespace Application.Commands;

public record CreateShareCommand(ShoppingListId ShoppingListId, UserId SharedWithUserId, SharePermission Permission);

public record UpdateShareCommand(ShareId Id, SharePermission Permission);

public record DeleteShareCommand(ShareId Id);