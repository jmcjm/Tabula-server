using Application.Commands;
using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Application.Interfaces;

public interface ITagService
{
    Task<ErrorOr<TagEntity>> CreateTagAsync(CreateTagCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<TagEntity>> UpdateTagAsync(UpdateTagCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteTagAsync(DeleteTagCommand command, UserId userId, CancellationToken cancellationToken = default);
}