using Legacy.DataAccess.Enums;

namespace Legacy.DataAccess.DTOs.Responses;

public record TagResponseModel(
    Guid Id,
    string Name,
    TagColor Color,
    DateTime CreatedAt,
    string UserId
); 