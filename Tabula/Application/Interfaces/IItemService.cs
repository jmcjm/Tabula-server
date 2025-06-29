using Application.Commands;
using Domain.Entities;
using Domain.Records;
using ErrorOr;

namespace Application.Interfaces;

public interface IItemService
{
    Task<ErrorOr<ItemEntity>> AddItemAsync(AddItemCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<ItemEntity>> UpdateItemAsync(UpdateItemCommand command, UserId userId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteItemAsync(DeleteItemCommand command, UserId userId, CancellationToken cancellationToken = default);
}