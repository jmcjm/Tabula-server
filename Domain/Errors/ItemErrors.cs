using ErrorOr;

namespace Domain.Errors;

public static class ItemErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        code: "ItemEntity.NotFound",
        description: $"ItemEntity with ID '{id}' was not found.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "ItemEntity.Validation",
        description: message);
} 