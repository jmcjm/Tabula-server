using System.Security.Claims;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Legacy.DataAccess.Enums;
using Legacy.DataAccess.Mappers;
using Legacy.DataAccess.Models;
using Tabula.Services.WebApi.Extensions;

namespace Tabula.Services.WebApi.Endpoints;

public static class TagsEndpoints
{
    public static void MapTagsEndpoints(this IEndpointRouteBuilder routes)
    {
        var tagRoutes = routes.MapGroup("/tags").WithTags("Tags");
        var shoppingListTagRoutes = routes.MapGroup("/shoppinglists").WithTags("Shopping List Tags");

        // Zarządzanie tagami użytkownika
        tagRoutes.MapGet("/", GetUserTags)
            .Produces<List<TagResponseModel>>()
            .Produces(500)
            .WithName("GetUserTags")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        tagRoutes.MapGet("/{id:Guid}", GetTag)
            .Produces<TagResponseModel>()
            .Produces(404)
            .Produces(500)
            .WithName("GetTag")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        tagRoutes.MapPost("/", CreateTag)
            .Produces<TagResponseModel>(201)
            .Produces(400)
            .Produces(500)
            .WithName("CreateTag")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        tagRoutes.MapDelete("/{id:Guid}", DeleteTag)
            .Produces(204)
            .Produces(404)
            .Produces(409) // Conflict - tag is used
            .Produces(500)
            .WithName("DeleteTag")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });
        
        shoppingListTagRoutes.MapPost("/{shoppingListId:Guid}/tags/{tagId:Guid}", AssignTagToShoppingList)
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(409) // Conflict - tag already assigned or limit reached
            .Produces(500)
            .WithName("AssignTagToShoppingList")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        shoppingListTagRoutes.MapDelete("/{shoppingListId:Guid}/tags/{tagId:Guid}", RemoveTagFromShoppingList)
            .Produces(204)
            .Produces(404)
            .Produces(500)
            .WithName("RemoveTagFromShoppingList")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });

        shoppingListTagRoutes.MapGet("/{shoppingListId:Guid}/available-tags", GetAvailableTagsForShoppingList)
            .Produces<List<TagResponseModel>>()
            .Produces(404)
            .Produces(500)
            .WithName("GetAvailableTagsForShoppingList")
            .RequireAuthorization(new AuthorizeAttribute
            {
                Roles = "admin,user",
                AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}"
            });
    }

    private static async Task<IResult> GetUserTags(
        [FromServices] ITagRepository tagRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var tags = await tagRepository.GetAllByUserIdAsync(currentUserId);
        var response = tags.Select(t => t.ToResponseModel()).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetTag(
        [FromRoute] Guid id,
        [FromServices] ITagRepository tagRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        var tag = await tagRepository.GetByIdAsync(id);

        if (tag.IsError)
            return tag.ToHttpResult();

        // Sprawdź czy tag należy do użytkownika
        if (tag.Value.UserId != currentUserId)
            return TypedResults.Forbid();

        return TypedResults.Ok(tag.Value.ToResponseModel());
    }

    private static async Task<IResult> CreateTag(
        [FromBody] CreateTagModel model,
        [FromServices] ITagRepository tagRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(model.Name))
            return TypedResults.BadRequest(new { error = "TagEntity name is required." });

        if (model.Name.Length > 50)
            return TypedResults.BadRequest(new { error = "TagEntity name cannot exceed 50 characters." });

        var tag = model.ToEntity(currentUserId);
        var addResult = await tagRepository.AddAsync(tag);
        
        return addResult.IsError 
            ? addResult.ToHttpResult() 
            : TypedResults.Created($"/tags/{tag.Id}", tag.ToResponseModel());
    }

    private static async Task<IResult> DeleteTag(
        [FromRoute] Guid id,
        [FromServices] ITagRepository tagRepository,
        ClaimsPrincipal user,
        [FromQuery] bool forceDelete = false)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        // Sprawdź, czy tag istnieje i należy do użytkownika
        var tagResult = await tagRepository.GetByIdAsync(id);
        if (tagResult.IsError)
            return tagResult.ToHttpResult();

        if (tagResult.Value.UserId != currentUserId)
            return TypedResults.Forbid();

        var deleteResult = await tagRepository.DeleteAsync(id, forceDelete);
        
        return deleteResult.IsError 
            ? deleteResult.ToHttpResult() 
            : TypedResults.NoContent();
    }

    private static async Task<IResult> AssignTagToShoppingList(
        [FromRoute] Guid shoppingListId,
        [FromRoute] Guid tagId,
        [FromServices] ITagRepository tagRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        // Sprawdź czy lista istnieje i użytkownik jest właścicielem
        var shoppingListResult = await shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingListResult.IsError)
            return shoppingListResult.ToHttpResult();

        var shoppingList = shoppingListResult.Value;
        if (shoppingList.UserId != currentUserId)
            return TypedResults.Forbid();

        // Sprawdź czy tag istnieje i należy do użytkownika
        var tagResult = await tagRepository.GetByIdAsync(tagId);
        if (tagResult.IsError)
            return tagResult.ToHttpResult();

        if (tagResult.Value.UserId != currentUserId)
            return TypedResults.BadRequest(new { error = "TagEntity does not belong to the current user." });

        var addResult = await tagRepository.AddToShoppingListAsync(shoppingListId, tagId);
        
        return addResult.IsError 
            ? addResult.ToHttpResult() 
            : TypedResults.NoContent();
    }

    private static async Task<IResult> RemoveTagFromShoppingList(
        [FromRoute] Guid shoppingListId,
        [FromRoute] Guid tagId,
        [FromServices] ITagRepository tagRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        // Sprawdź czy lista istnieje i użytkownik jest właścicielem
        var shoppingListResult = await shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingListResult.IsError)
            return shoppingListResult.ToHttpResult();

        var shoppingList = shoppingListResult.Value;
        if (shoppingList.UserId != currentUserId)
            return TypedResults.Forbid();

        var removeResult = await tagRepository.RemoveFromShoppingListAsync(shoppingListId, tagId);
        
        return removeResult.IsError 
            ? removeResult.ToHttpResult() 
            : TypedResults.NoContent();
    }

    private static async Task<IResult> GetAvailableTagsForShoppingList(
        [FromRoute] Guid shoppingListId,
        [FromServices] ITagRepository tagRepository,
        [FromServices] IShoppingListRepository shoppingListRepository,
        [FromServices] IShareRepository shareRepository,
        ClaimsPrincipal user)
    {
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return TypedResults.Unauthorized();

        try
        {
            // Sprawdź czy użytkownik ma dostęp do listy (Read wystarczy)
            if (!await shareRepository.HasPermissionAsync(currentUserId, shoppingListId, SharePermission.ReadOnly))
                return TypedResults.Forbid();

            // Pobierz listę żeby znaleźć właściciela
            var shoppingListResult = await shoppingListRepository.GetByIdAsync(shoppingListId);
            if (shoppingListResult.IsError)
                return shoppingListResult.ToHttpResult();

            var shoppingList = shoppingListResult.Value;

            // Zwróć tagi właściciela listy
            var ownerTags = await tagRepository.GetAllByUserIdAsync(shoppingList.UserId);
            var response = ownerTags.Select(t => t.ToResponseModel()).ToList();
            return TypedResults.Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}