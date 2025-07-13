using Domain.Records;

namespace Application.Commands;

public record CreateShoppingListCommand(string Name, List<TagId> Tags);

public record UpdateShoppingListCommand(ShoppingListId Id, string Name);

public record DeleteShoppingListCommand(ShoppingListId Id);

public record AddItemCommand(string ProductName, ushort Quantity, bool Bought, ShoppingListId ShoppingListId);

public record UpdateItemCommand(ItemId Id, string ProductName, ushort Quantity, bool Bought);

public record DeleteItemCommand(ItemId Id);

public record AssignTagCommand(ShoppingListId ShoppingListId, TagId TagId);

public record RemoveTagCommand(ShoppingListId ShoppingListId, TagId TagId);