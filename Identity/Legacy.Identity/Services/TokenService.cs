using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Legacy.Identity.Interfaces;
using Legacy.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Legacy.Identity.Services;

public class TokenService(IConfiguration configuration, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : ITokenService
{
    public async Task<string> ValidateUser(LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);

        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return "";

        var roles = await userManager.GetRolesAsync(user);
        return GenerateToken(user, roles);
    }
    
    public async Task<IdentityResult> CreateUserAsync(RegisterModel model)
    {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        
        var result = await userManager.CreateAsync(user, model.Password);
        
        if (!result.Succeeded)
            return result;

        return await AssignRolesAsync(user, model.Roles);
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
            return IdentityResult.Failed(new IdentityError { Description = "Role already exists." });

        return await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    public async Task<IdentityResult> AssignRoleAsync(string username, string role)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null || !await roleManager.RoleExistsAsync(role))
            return IdentityResult.Failed();

        return await userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveRoleAsync(string username, string role)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
            return IdentityResult.Failed();

        return await userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<IList<string>> GetUserRolesAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);
        return user == null ? [] : await userManager.GetRolesAsync(user);
    }

    public string[] GetRoles(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            return [];

        var rolesClaims = jwtToken.Claims
            .Where(x => x.Type is ClaimTypes.Role)
            .Select(x => x.Value);

        return rolesClaims.ToArray();
    }

    public async Task<IdentityResult> AssignRolesAsync(IdentityUser user, IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                continue;

            var roleResult = await userManager.AddToRoleAsync(user, role);
            
            if (roleResult.Succeeded) continue;
            
            await userManager.DeleteAsync(user);
            return roleResult;
        }
        return IdentityResult.Success;
    }
    
    public async Task<bool> DoesUserExistAsync(string username)
    {
        return await userManager.FindByNameAsync(username) != null;
    }
    
    private string GenerateToken(IdentityUser user, IList<string> roles)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.UserName!),
            new (ClaimTypes.NameIdentifier, user.Id),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (ClaimTypes.AuthenticationMethod, "tabula-jwt")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var expiresInMinutes = jwtSettings["ExpiresInMinutes"] != null ? 
            Convert.ToDouble(jwtSettings["ExpiresInMinutes"]) : 60;
        
        if (expiresInMinutes <= 0)
            expiresInMinutes = 60;
            
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
