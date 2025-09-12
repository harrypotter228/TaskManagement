using TaskManagement.Server.Contracts.DtoEnums;

namespace TaskManagement.Server.Contracts.Responses
{
    public sealed record UpdateBoardStatusesResponse(Guid BoardId, IReadOnlyList<TaskStatusDtoEnum> Statuses);
}
