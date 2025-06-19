using System.Security.Claims;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Legacy.DataAccess.Enums;

namespace Tabula.Services.WebApi.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/byShoppingList/{shoppingListId:Guid}", GetItems)
            .Produces<List<ItemEntity>>()
            .Produces(500)
            .WithName("GetItems")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapGet("/{id:Guid}", GetItem)
            .Produces<ItemEntity>()
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
            .Produces<ItemEntity>(201)
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
        [FromServices] IShareRepository shareRepository,
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
        [FromBody] ItemEntity itemEntity,
        [FromServices] IItemRepository itemRepository)
    {
        if (string.IsNullOrEmpty(itemEntity.ProductName))
            return TypedResults.BadRequest(new { error = "Product name is required." });

        if (itemEntity.Quantity <= 0)
            return TypedResults.BadRequest(new { error = "Quantity must be greater than 0." });

        if (itemEntity.ShoppingListId == Guid.Empty)
            return TypedResults.BadRequest(new { error = "Shopping list ID is required." });

        await itemRepository.AddAsync(itemEntity);
        return TypedResults.Created($"/items/{itemEntity.Id}", itemEntity);
    }

    private static async Task<IResult> PutItem(
        [FromRoute] Guid id,
        [FromBody] ItemEntity itemEntity,
        [FromServices] IItemRepository itemRepository)
    {
        if (string.IsNullOrEmpty(itemEntity.ProductName))
            return TypedResults.BadRequest(new { error = "Product name is required." });

        if (itemEntity.Quantity <= 0)
            return TypedResults.BadRequest(new { error = "Quantity must be greater than 0." });

        if (itemEntity.ShoppingListId == Guid.Empty)
            return TypedResults.BadRequest(new { error = "Shopping list ID is required." });

        await itemRepository.UpdateAsync(itemEntity);
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