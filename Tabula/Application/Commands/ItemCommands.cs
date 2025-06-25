using Domain.Records;

namespace Application.Commands;

public record AddItemCommand(string ProductName, ushort Quantity, bool Bought, ShoppingListId ShoppingListId);

public record UpdateItemCommand(ItemId Id, string ProductName, ushort Quantity, bool Bought);

public record DeleteItemCommand(ItemId Id);