using System.Security.Claims;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Legacy.DataAccess.Enums;
using Legacy.DataAccess.Models;
using Tabula.Services.WebApi.Extensions;

namespace Tabula.Services.WebApi.Endpoints;

public static class SharingEndpoints
{
    public static void MapSharingEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/share", ShareShoppingList)
            .Produces<ShareResponseModel>(201)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(500)
            .WithName("ShareShoppingList")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapDelete("/share/{shareId:Guid}", RevokeShare)
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500)
            .WithName("RevokeShare")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapGet("/shared-with-me", GetSharedWithMe)
            .Produces<List<ShareResponseModel>>()
            .Produces(401)
            .Produces(500)
            .WithName("GetSharedWithMe")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapGet("/shared-by-me", GetSharedByMe)
            .Produces<List<ShareResponseModel>>()
            .Produces(401)
            .Produces(500)
            .WithName("GetSharedByMe")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapGet("/list/{shoppingListId:Guid}/shares", GetListShares)
            .Produces<List<ShareResponseModel>>()
            .Produces(401)
            .Produces(403)
            .Produces(500)
            .WithName("GetListShares")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });

        routes.MapPut("/share/{shareId:Guid}/permission", UpdateSharePermission)
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500)
            .WithName("UpdateSharePermission")
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    Roles = "admin,user",
                    AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
                });
    }

    private static async Task<IResult> ShareShoppingList(
        [FromBody] ShareRequestModel request,
        [FromServices] IShareRepository shareRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] UserManager<IdentityUser> userManager,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        // Sprawdź czy lista istnieje i czy użytkownik jest jej właścicielem
        var shoppingListResult = await shoppingListRepository.GetByIdAsync(request.ShoppingListId);
        if (shoppingListResult.IsError)
            return shoppingListResult.ToHttpResult();
        
        var shoppingList = shoppingListResult.Value;
        if (shoppingList.UserId != currentUserId)
            return TypedResults.Forbid();

        // Znajdź użytkownika po username
        var targetUser = await userManager.FindByNameAsync(request.SharedWithUsername);
        if (targetUser == null)
            return TypedResults.BadRequest(new { error = "User not found." });

        // Sprawdź czy użytkownik nie próbuje udostępnić listy samemu sobie
        if (targetUser.Id == currentUserId)
            return TypedResults.BadRequest(new { error = "Cannot share list with yourself." });

        // Sprawdź czy udostępnienie już nie istnieje
        var existingShare = await shareRepository.GetShareAsync(request.ShoppingListId, targetUser.Id);
        if (existingShare != null)
            return TypedResults.BadRequest(new { error = "List is already shared with this user." });

        var share = new ShareEntity
        {
            ShoppingListId = request.ShoppingListId,
            OwnerId = currentUserId,
            SharedWithUserId = targetUser.Id,
            Permission = request.Permission,
            SharedAt = DateTime.UtcNow
        };

        await shareRepository.AddAsync(share);

        // Pobierz username właściciela
        var owner = await userManager.FindByIdAsync(currentUserId);
        var ownerUsername = owner?.UserName ?? "Unknown";

        var response = new ShareResponseModel(
            share.Id,
            share.ShoppingListId,
            shoppingList.Name,
            ownerUsername,
            targetUser.UserName ?? "Unknown",
            share.Permission,
            share.SharedAt
        );

        return TypedResults.Created($"/sharing/share/{share.Id}", response);
    }

    private static async Task<IResult> RevokeShare(
        [FromRoute] Guid shareId,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var share = await shareRepository.GetByIdAsync(shareId);
        
        if (share.IsError)
            return share.ToHttpResult();
        
        // Only owner can revoke access
        if (share.Value.OwnerId != currentUserId)
            return TypedResults.Forbid();

        var result = await shareRepository.DeleteAsync(shareId);
        
        return result.IsError ? result.ToHttpResult() : TypedResults.Ok();
    }

    private static async Task<IResult> GetSharedWithMe(
        [FromServices] IShareRepository shareRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] UserManager<IdentityUser> userManager,
        [FromServices] ILogger logger,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var shares = await shareRepository.GetSharedWithUserAsync(currentUserId);
        
        var response = new List<ShareResponseModel>();
        foreach (var share in shares)
        {
            var shoppingListResult = await shoppingListRepository.GetByIdAsync(share.ShoppingListId);
            
            if (shoppingListResult.IsError)
                continue;
            
            var shoppingList = shoppingListResult.Value;
            
            var owner = await userManager.FindByIdAsync(share.OwnerId);
            var sharedWith = await userManager.FindByIdAsync(share.SharedWithUserId);

            if (owner == null)
            {
                // If the owner is null, something went wrong, log it and skip this share
                logger.LogError("Owner username of {ShoppingListName} {ShoppingListId} is null.", shoppingList.Name, shoppingList.Id);
                continue;
            }
            
            if (sharedWith == null)
            {
                // If we got null in here... Well, that's extremely weird - we definitely have to log it
                logger.LogError("SharedWith username of {ShoppingListName} {ShoppingListId} is null.", shoppingList.Name, shoppingList.Id);
                continue;
            }
            
            response.Add(new ShareResponseModel(
                share.Id,
                share.ShoppingListId,
                shoppingList.Name,
                owner.UserName!,
                sharedWith.UserName!,
                share.Permission,
                share.SharedAt
            ));
        }

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetSharedByMe(
        [FromServices] IShareRepository shareRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] UserManager<IdentityUser> userManager,
        [FromServices] ILogger logger,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var shares = await shareRepository.GetSharedByUserAsync(currentUserId);
        
        var response = new List<ShareResponseModel>();
        foreach (var share in shares)
        {
            var shoppingListResult = await shoppingListRepository.GetByIdAsync(share.ShoppingListId);
            if (shoppingListResult.IsError)
                continue;
            
            var shoppingList = shoppingListResult.Value;
            
            var owner = await userManager.FindByIdAsync(share.OwnerId);
            var sharedWith = await userManager.FindByIdAsync(share.SharedWithUserId);

            if (owner == null)
            {
                // If the owner is null, something went wrong, log it and skip this share
                logger.LogError("Owner username of {ShoppingListName} {ShoppingListId} is null.", shoppingList.Name, shoppingList.Id);
                continue;
            }
            
            if (sharedWith == null)
            {
                // If we got null in here... Well, that's extremely weird - we definitely have to log it
                logger.LogError("SharedWith username of {ShoppingListName} {ShoppingListId} is null.", shoppingList.Name, shoppingList.Id);
                continue;
            }
            
            response.Add(new ShareResponseModel(
                share.Id,
                share.ShoppingListId,
                shoppingList.Name,
                owner.UserName!,
                sharedWith.UserName!,
                share.Permission,
                share.SharedAt
            ));
        }

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetListShares(
        [FromRoute] Guid shoppingListId,
        [FromServices] IShareRepository shareRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] UserManager<IdentityUser> userManager,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();
        
        if (!await shareRepository.HasPermissionAsync(currentUserId, shoppingListId, SharePermission.ReadOnly))
            return TypedResults.Forbid();

        var shares = await shareRepository.GetSharesForShoppingListAsync(shoppingListId);
        var shoppingListResult = await shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingListResult.IsError)
            return shoppingListResult.ToHttpResult();
        
        var shoppingList = shoppingListResult.Value;
        
        var response = new List<ShareResponseModel>();
        foreach (var share in shares)
        {
            // Pobierz usernames
            var owner = await userManager.FindByIdAsync(share.OwnerId);
            var sharedWith = await userManager.FindByIdAsync(share.SharedWithUserId);
            
            response.Add(new ShareResponseModel(
                share.Id,
                share.ShoppingListId,
                shoppingList.Name,
                owner?.UserName ?? "Unknown",
                sharedWith?.UserName ?? "Unknown",
                share.Permission,
                share.SharedAt
            ));
        }

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> UpdateSharePermission(
        [FromRoute] Guid shareId,
        [FromBody] SharePermission newPermission,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var share = await shareRepository.GetByIdAsync(shareId);
        
        if (share.IsError)
            return share.ToHttpResult();
        
        // Only owner can change permissions
        if (share.Value.OwnerId != currentUserId)
            return TypedResults.Forbid();

        share.Value.Permission = newPermission;
        await shareRepository.UpdateAsync(share.Value);

        return TypedResults.NoContent();
    }
} 