namespace Tabula.Infrastructure.Identity.Models;

public class RoleAuthorizationMetadata(params string[] roles)
{
    public string[] Roles { get; } = roles;
}