using Domain.Records;

namespace Application.Commands;

public record CreateShoppingListCommand(string Name);

public record UpdateShoppingListCommand(ShoppingListId Id, string Name);

public record DeleteShoppingListCommand(ShoppingListId Id);