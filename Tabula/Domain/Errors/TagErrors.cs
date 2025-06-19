using ErrorOr;

namespace Domain.Errors;

public static class TagErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        code: "TagEntity.NotFound",
        description: $"TagEntity with ID '{id}' was not found.");

    public static Error AlreadyExists(string name) => Error.Conflict(
        code: "TagEntity.AlreadyExists",
        description: $"TagEntity with name '{name}' already exists for this user.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "TagEntity.Validation",
        description: message);

    public static Error LimitExceeded(int maxTags = 5) => Error.Validation(
        code: "TagEntity.LimitExceeded",
        description: $"Cannot assign more than {maxTags} tags to a shopping list.");

    public static Error InUse(Guid tagId) => Error.Conflict(
        code: "TagEntity.InUse",
        description: $"TagEntity with ID '{tagId}' is currently in use and cannot be deleted.");
} 