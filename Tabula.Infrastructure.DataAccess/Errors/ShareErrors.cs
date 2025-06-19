using ErrorOr;

namespace Tabula.Infrastructure.DataAccess.Errors;

public static class ShareErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        code: "Share.NotFound",
        description: $"Share with ID '{id}' was not found.");

    public static Error AlreadyShared(string username) => Error.Conflict(
        code: "Share.AlreadyShared",
        description: $"List is already shared with user '{username}'.");

    public static Error CannotShareWithSelf => Error.Validation(
        code: "Share.CannotShareWithSelf",
        description: "Cannot share list with yourself.");

    public static Error UserNotFound(string username) => Error.NotFound(
        code: "Share.UserNotFound",
        description: $"User '{username}' was not found.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "Share.Validation",
        description: message);
} 