using ErrorOr;

namespace Domain.Errors;

public static class ShoppingListErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        code: "ShoppingList.NotFound",
        description: $"Shopping list with ID '{id}' was not found.");

    public static Error AlreadyExists(string name) => Error.Conflict(
        code: "ShoppingList.AlreadyExists", 
        description: $"Shopping list with name '{name}' already exists.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "ShoppingList.Validation",
        description: message);
} 