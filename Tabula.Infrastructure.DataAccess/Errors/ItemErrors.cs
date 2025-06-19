using ErrorOr;

namespace Tabula.Infrastructure.DataAccess.Errors;

public static class ItemErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        code: "Item.NotFound",
        description: $"Item with ID '{id}' was not found.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "Item.Validation",
        description: message);
} 