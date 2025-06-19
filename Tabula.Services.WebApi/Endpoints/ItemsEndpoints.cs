using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tabula.Infrastructure.DataAccess.Entities;
using Tabula.Infrastructure.DataAccess.Enums;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Services.WebApi.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/byShoppingList/{shoppingListId:Guid}", GetItems)
            .Produces<List<Item>>()
            .Produces(500)
            .WithName("GetItems")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapGet("/{id:Guid}", GetItem)
            .Produces<Item>()
            .Produces(404)
            .Produces(500)
            .WithName("GetItem")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapPost("/", PostItem)
            .Produces<Item>(201)
            .Produces(400)
            .Produces(500)
            .WithName("PostItem")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapPut("/{id:Guid}", PutItem)
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500)
            .WithName("PutItem")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapDelete("/{id:Guid}", DeleteItem)
            .Produces(204)
            .Produces(404)
            .Produces(500)
            .WithName("DeleteItem")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });
    }

    private static async Task<IResult> GetItems(
        [FromRoute] Guid shoppingListId,
        [FromServices] IItemRepository itemRepository,
        [FromServices] IShoppingListShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();
        
        if (!await shareRepository.HasPermissionAsync(currentUserId, shoppingListId, SharePermission.ReadOnly))
            return TypedResults.Forbid();

        var items = await itemRepository.GetAllByShoppingListIdAsync(shoppingListId);
        return TypedResults.Ok(items);
    }

    private static async Task<IResult> GetItem(
        [FromRoute] Guid id,
        [FromServices] IItemRepository itemRepository)
    {
        var item = await itemRepository.GetByIdAsync(id);
        return TypedResults.Ok(item);
    }

    private static async Task<IResult> PostItem(
        [FromBody] Item item,
        [FromServices] IItemRepository itemRepository)
    {
        if (string.IsNullOrEmpty(item.ProductName))
            return TypedResults.BadRequest(new { error = "Product name is required." });

        if (item.Quantity <= 0)
            return TypedResults.BadRequest(new { error = "Quantity must be greater than 0." });

        if (item.ShoppingListId == Guid.Empty)
            return TypedResults.BadRequest(new { error = "Shopping list ID is required." });

        await itemRepository.AddAsync(item);
        return TypedResults.Created($"/items/{item.Id}", item);
    }

    private static async Task<IResult> PutItem(
        [FromRoute] Guid id,
        [FromBody] Item item,
        [FromServices] IItemRepository itemRepository)
    {
        if (string.IsNullOrEmpty(item.ProductName))
            return TypedResults.BadRequest(new { error = "Product name is required." });

        if (item.Quantity <= 0)
            return TypedResults.BadRequest(new { error = "Quantity must be greater than 0." });

        if (item.ShoppingListId == Guid.Empty)
            return TypedResults.BadRequest(new { error = "Shopping list ID is required." });

        await itemRepository.UpdateAsync(item);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> DeleteItem(
        [FromRoute] Guid id,
        [FromServices] IItemRepository itemRepository)
    {
        await itemRepository.DeleteAsync(id);
        return TypedResults.NoContent();
    }
} 