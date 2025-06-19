using ErrorOr;

namespace Tabula.Infrastructure.DataAccess.Errors;

public static class UserErrors
{
    public static Error NotFound(string identifier) => Error.NotFound(
        code: "User.NotFound",
        description: $"User '{identifier}' was not found.");

    public static Error ValidationError(string message) => Error.Validation(
        code: "User.Validation",
        description: message);
} 