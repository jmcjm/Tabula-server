namespace Tabula.Infrastructure.Identity.Models;

public class AssignRoleModel(string username, string role)
{
    public string Username { get; } = username;
    public string Role { get; } = role;
}
