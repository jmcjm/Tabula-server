using System.Security.Claims;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Legacy.DataAccess.Enums;
using Tabula.Services.WebApi.Extensions;

namespace Tabula.Services.WebApi.Endpoints;

public static class ShoppingListsEndpoints
{
    public static void MapShoppingListsEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/byUser/{userId}", GetShoppingLists)
            .Produces<List<ShoppingListEntity>>()
            .Produces(500)
            .WithName("GetShoppingLists")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapGet("/{id:Guid}", GetShoppingList)
            .Produces<ShoppingListEntity>()
            .Produces(404)
            .Produces(500)
            .WithName("GetShoppingList")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapPost("/", PostShoppingList)
            .Produces<ShoppingListEntity>(201)
            .Produces(400)
            .Produces(500)
            .WithName("PostShoppingList")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapPut("/{id:Guid}", PutShoppingList)
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500)
            .WithName("PutShoppingList")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapDelete("/{id:Guid}", DeleteShoppingList)
            .Produces(204)
            .Produces(404)
            .Produces(500)
            .WithName("DeleteShoppingList")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });
    }

    private static async Task<IResult> GetShoppingLists(
        [FromRoute] string userId,
        [FromServices] IShoppingListRepository shoppingListRepository)
    {
        var shoppingLists = await shoppingListRepository
            .GetAllByUserAsync(userId);
        
        return TypedResults.Ok(shoppingLists);
    }

    private static async Task<IResult> GetShoppingList(
        [FromRoute] Guid id,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var shoppingList = await shoppingListRepository.GetByIdAsync(id);
        
        if (shoppingList.IsError)
            return shoppingList.ToHttpResult();
        
        if (shoppingList.Value.UserId == currentUserId)
            return TypedResults.Ok(shoppingList.Value);

        if (await shareRepository.HasPermissionAsync(currentUserId, id, SharePermission.ReadOnly))
            return TypedResults.Ok(shoppingList.Value);
        
        return TypedResults.Forbid();
    }

    private static async Task<IResult> PostShoppingList(
        [FromBody] ShoppingListEntity shoppingList,
        [FromServices] IShoppingListRepository shoppingListRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        if (string.IsNullOrEmpty(shoppingList.Name))
            return TypedResults.BadRequest(new { error = "Shopping list name is required." });

        // Set owner of the list to the current user
        shoppingList.UserId = currentUserId;

        return await shoppingListRepository
            .AddAsync(shoppingList)
            .ToHttpResultAsync(_ => TypedResults.Created($"/shoppinglists/{shoppingList.Id}", shoppingList));
    }

    private static async Task<IResult> PutShoppingList(
        [FromRoute] Guid id, 
        [FromBody] ShoppingListEntity shoppingList,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();
        
        if (!await shareRepository.HasPermissionAsync(currentUserId, id, SharePermission.ReadWrite))
            return TypedResults.Forbid();

        return await shoppingListRepository
            .UpdateAsync(shoppingList)
            .ToHttpResultAsync(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> DeleteShoppingList(
        [FromRoute] Guid id,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        // Check if user is owner of the list
        var shoppingListResult = await shoppingListRepository.GetByIdAsync(id);
        if (shoppingListResult.IsError)
            return shoppingListResult.ToHttpResult();

        if (shoppingListResult.Value.UserId != currentUserId)
            return TypedResults.Forbid();

        return await shoppingListRepository
            .DeleteAsync(id)
            .ToHttpResultAsync(_ => TypedResults.NoContent());
    }
}