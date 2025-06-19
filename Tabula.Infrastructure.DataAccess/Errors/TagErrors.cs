using ErrorOr;

namespace Tabula.Infrastructure.DataAccess.Errors;

public static class TagErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        code: "Tag.NotFound",
        description: $"Tag with ID '{id}' was not found.");

    public static Error AlreadyExists(string name) => Error.Conflict(
        code: "Tag.AlreadyExists",
        description: $"Tag with name '{name}' already exists for this user.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "Tag.Validation",
        description: message);

    public static Error LimitExceeded(int maxTags = 5) => Error.Validation(
        code: "Tag.LimitExceeded",
        description: $"Cannot assign more than {maxTags} tags to a shopping list.");

    public static Error InUse(Guid tagId) => Error.Conflict(
        code: "Tag.InUse",
        description: $"Tag with ID '{tagId}' is currently in use and cannot be deleted.");
} 