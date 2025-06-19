using ErrorOr;

namespace Tabula.Infrastructure.DataAccess.Errors;

public static class PermissionErrors
{
    public static Error AccessDenied(string operation) => Error.Forbidden(
        code: "Permission.AccessDenied",
        description: $"You don't have permission to perform '{operation}' on this resource.");

    public static Error InsufficientRights(string requiredPermission) => Error.Forbidden(
        code: "Permission.InsufficientRights",
        description: $"Operation requires '{requiredPermission}' permission.");
} 