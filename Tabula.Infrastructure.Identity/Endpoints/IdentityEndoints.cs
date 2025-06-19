using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using Tabula.Infrastructure.Identity.Interfaces;
using Tabula.Infrastructure.Identity.Models;

namespace Tabula.Infrastructure.Identity.Endpoints;

public static class IdentityEndoints
{
    public static void MapIdentityEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/login", Login)
            .Produces<TokenResponse>()
            .Produces(400)
            .Produces(401)
            .Produces(500)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Pozwala na zalogowanie się za pomocą loginu i hasła."
            })
            .WithName("Login");

        routes.MapPost("/register", CreateUser)
            .Produces(200)
            .Produces<ProblemDetails>(400)
            .Produces(401)
            .Produces(500)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Tworzy nowego użytkowika"
            })
            .WithName("CreateUser");

        routes.MapPost("/roles", CreateRole)
            .Produces(200)
            .Produces<ProblemDetails>(400)
            .Produces(500)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Tworzy nową rolę"
            })
            .WithName("CreateRole")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        routes.MapPost("/roles/assign", AssignRole)
            .Produces(200)
            .Produces<ProblemDetails>(400)
            .Produces(500)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Przypisuje rolę użytkownikowi"
            })
            .WithName("AssignRole")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        routes.MapPost("/roles/remove", RemoveRole)
            .Produces(200)
            .Produces<ProblemDetails>(400)
            .Produces(500)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Usuwa rolę od użytkownika"
            })
            .WithName("RemoveRole")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        routes.MapGet("/users/{username}/roles", GetUserRoles)
            .Produces<IList<string>>()
            .Produces<ProblemDetails>(404)
            .Produces(500)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Pobiera role przypisane do użytkownika"
            })
            .WithName("GetUserRoles")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });
    }

    private static async Task<IResult> Login([FromBody] LoginModel model, [FromServices] ITokenService tokenService)
    {
        var token = await tokenService.ValidateUser(model);

        return string.IsNullOrEmpty(token) ? TypedResults.Unauthorized() : TypedResults.Ok(new { Token = token });
    }

    private static async Task<IResult> CreateUser([FromBody] RegisterModel model,
        [FromServices] ITokenService tokenService)
    {
        var result = await tokenService.CreateUserAsync(model);

        return result.Succeeded
            ? TypedResults.Ok("User created successfully.")
            : TypedResults.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "User creation failed",
                detail: string.Join(" ", result.Errors.Select(e => e.Description))
            );
    }

    private static async Task<IResult> CreateRole([FromBody] string roleName, [FromServices] ITokenService tokenService)
    {
        var result = await tokenService.CreateRoleAsync(roleName);
        return result.Succeeded
            ? TypedResults.Ok("Role created successfully.")
            : TypedResults.Problem("Role creation failed");
    }

    private static async Task<IResult> AssignRole([FromBody] AssignRoleModel model,
        [FromServices] ITokenService tokenService)
    {
        var result = await tokenService.AssignRoleAsync(model.Username, model.Role);
        return result.Succeeded
            ? TypedResults.Ok("Role assigned successfully.")
            : TypedResults.Problem("Failed to assign role");
    }

    private static async Task<IResult> RemoveRole([FromBody] AssignRoleModel model,
        [FromServices] ITokenService tokenService)
    {
        var result = await tokenService.RemoveRoleAsync(model.Username, model.Role);
        return result.Succeeded
            ? TypedResults.Ok("Role removed successfully.")
            : TypedResults.Problem("Failed to remove role");
    }

    private static async Task<IResult> GetUserRoles(string username, [FromServices] ITokenService tokenService)
    {
        var roles = await tokenService.GetUserRolesAsync(username);
        return roles.Count > 0 ? TypedResults.Ok(roles) : TypedResults.NotFound("User not found or has no roles.");
    }
}