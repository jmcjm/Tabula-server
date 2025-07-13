using System.Security.Claims;
using Domain.Records;
using ErrorOr;

namespace Presentation.Helpers;

internal static class NameGetter
{
    internal static ErrorOr<UserId> GetNameIdentifier(this ClaimsPrincipal user)
    {
        var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(nameIdentifier))
            return Error.Unauthorized("nameIdentifier", "nameIdentifier claim is missing.");

        return new UserId(nameIdentifier);
    }
}