using Domain.Enums;
using Domain.Records;

namespace Application.Commands;

public record CreateTagCommand(string Name, TagColor Color);

public record UpdateTagCommand(TagId Id, string Name, TagColor Color);

public record DeleteTagCommand(TagId Id);