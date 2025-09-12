using TaskManagement.Server.Contracts.DtoEnums;

namespace TaskManagement.Server.Contracts.Dtos
{
    public sealed record TaskDto(
    Guid Id,
    string Name,
    string? Description,
    string? Deadline,
    TaskStatusDtoEnum Status,
    Guid BoardId,
    bool IsFavorite
);
}
