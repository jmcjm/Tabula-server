using Legacy.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Legacy.Identity.Interfaces;

public interface ITokenService
{
    Task<string> ValidateUser(LoginModel model);
    Task<IdentityResult> CreateUserAsync(RegisterModel model);
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> AssignRoleAsync(string username, string role);
    Task<IdentityResult> RemoveRoleAsync(string username, string role);
    Task<IList<string>> GetUserRolesAsync(string username);
    string[] GetRoles(string token);
    Task<IdentityResult> AssignRolesAsync(IdentityUser user, IEnumerable<string> roles);
    Task<bool> DoesUserExistAsync(string username);
}