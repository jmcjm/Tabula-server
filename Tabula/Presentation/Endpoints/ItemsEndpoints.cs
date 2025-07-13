using System.Security.Claims;
using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Records;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace Presentation.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoint(this WebApplication app)
    {
        var itemsGroup = app.MapGroup("api/v1/items")
            .WithTags("Items")
            .WithName("Items");

        itemsGroup.MapPost("", PostItem)
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

        itemsGroup.MapPut("/{id:Guid}", PutItem)
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

        itemsGroup.MapDelete("/{id:Guid}", DeleteItem)
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

    private static async Task<IResult> PostItem(
        [FromBody] AddItemCommand command,
        [FromServices] IShoppingListService shoppingListService,
        ClaimsPrincipal user)
    {
        var currentUserIdResult = user.GetNameIdentifier();

        if (currentUserIdResult.IsError)
            return ErrorMapper.MapErrorsToProblemResponse(currentUserIdResult, "Unauthorized");
        
        var result = await shoppingListService.AddItemToShoppingListAsync(command, currentUserIdResult.Value);
        return result.IsError ? ErrorMapper.MapErrorsToProblemResponse(result, "AddItem") : Results.Ok();
    }

    private static async Task<IResult> PutItem(
        [FromRoute] Guid id,
        [FromBody] UpdateItemCommand command,
        [FromServices] IShoppingListService shoppingListService,
        ClaimsPrincipal user)
    {
        var currentUserIdResult = user.GetNameIdentifier();

        if (currentUserIdResult.IsError)
            return ErrorMapper.MapErrorsToProblemResponse(currentUserIdResult, "Unauthorized");
        
        var result = await shoppingListService.UpdateItemInShoppingListAsync(command with { Id = new ItemId(id) }, currentUserIdResult.Value);
        return result.IsError ? ErrorMapper.MapErrorsToProblemResponse(result, "UpdateItem") : Results.NoContent();
    }

    private static async Task<IResult> DeleteItem(
        [FromRoute] Guid id,
        [FromServices] IShoppingListService shoppingListService,
        ClaimsPrincipal user)
    {
        var currentUserIdResult = user.GetNameIdentifier();

        if (currentUserIdResult.IsError)
            return ErrorMapper.MapErrorsToProblemResponse(currentUserIdResult, "Unauthorized");
        
        var command = new DeleteItemCommand(new ItemId(id));
        var result = await shoppingListService.DeleteItemFromShoppingListAsync(command, currentUserIdResult.Value);
        return result.IsError ? ErrorMapper.MapErrorsToProblemResponse(result, "DeleteItem") : Results.NoContent();
    }
}