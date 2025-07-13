using Domain.Entities;
using Domain.Interfaces;
using Domain.Records;
using ErrorOr;
using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.EfRepositories;

public class TagRepository(TabulaDbContext context, ILogger<TagRepository> logger) : ITagRepository
{
    public Task<List<TagEntity>> GetAllByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<TagEntity>> GetByIdAsync(TagId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<TagEntity>> GetByIdAndUserIdAsync(TagId id, UserId userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<Success>> AddAsync(TagEntity tagEntity, CancellationToken cancellationToken = default)
    {
        var dbTag = new TagDbModel
        {
            Id = tagEntity.Id.Value,
            Name = tagEntity.Name,
            Color = (int)tagEntity.Color,
            CreatedAt = tagEntity.CreatedAt,
            UserId = tagEntity.UserId.Value
        };

        context.Tags.Add(dbTag);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            context.Entry(dbTag).State = EntityState.Detached;
            return Error.Conflict("Tag.AlreadyExists", "The tag already exists.");
        }
        catch (DbUpdateException ex)
        {
            context.Entry(dbTag).State = EntityState.Detached;
            logger.LogError(ex, "Unexpected DB error while adding tag {TagId}", tagEntity.Id);
            return Error.Unexpected(description: "Failed to save Tag.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while adding tag {TagId}: {msg}", tagEntity.Id, ex.Message);
            throw;
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is NpgsqlException { SqlState: "23505" };
    }

    public Task<ErrorOr<Success>> DeleteAsync(TagId id, bool forceDelete = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> UpdateAsync(TagEntity tagEntity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TagEntity>> GetTagsByShoppingListIdAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTagUsedAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> AddToShoppingListAsync(ShoppingListId shoppingListId, TagId tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> RemoveFromShoppingListAsync(ShoppingListId shoppingListId, TagId tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTagCountForShoppingListAsync(ShoppingListId shoppingListId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}