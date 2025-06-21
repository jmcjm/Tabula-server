using Application.Commands;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Application.Services;

public class TagService(ITagRepository tagRepository)
{
    public async Task<ErrorOr<TagEntity>> CreateTagAsync(CreateTagCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        // We don't care about possible duplicates here (same UserId and Name) as we're using unique index on db so we will catch them on INSERT
        var creationResult = TagEntity.Create(command.Name, command.Color, userId);
        if (creationResult.IsError)
            return creationResult.Errors;
        
        var tagEntity = creationResult.Value;

        var addResult = await tagRepository.AddAsync(tagEntity, cancellationToken);
        return addResult.IsError ? addResult.Errors : tagEntity;
    }
    
    public async Task<ErrorOr<TagEntity>> UpdateTagAsync(UpdateTagCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        // We query the DB by Id and UserId (which we supply from JWT),
        // so the user cannot access or update tags they do not own.
        var tagResult = await tagRepository.GetByIdAndUserIdAsync(command.Id, userId, cancellationToken);
        if (tagResult.IsError)
            return tagResult.Errors;

        var tag = tagResult.Value;

        var nameUpdateResult = tag.UpdateName(command.Name);
        if (nameUpdateResult.IsError)
            return nameUpdateResult.Errors;

        tag.UpdateColor(command.Color);

        var updateResult = await tagRepository.UpdateAsync(tag, cancellationToken);
        return updateResult.IsError ? updateResult.Errors : tag;
    }

    
    public async Task<ErrorOr<Deleted>> DeleteTagAsync(DeleteTagCommand command, UserId userId, CancellationToken cancellationToken = default)
    {
        var tagResult = await tagRepository.GetByIdAndUserIdAsync(command.Id, userId, cancellationToken);
        if (tagResult.IsError)
            return tagResult.Errors;

        var deleteResult = await tagRepository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
        return deleteResult.IsError ? deleteResult.Errors : Result.Deleted;
    }
}