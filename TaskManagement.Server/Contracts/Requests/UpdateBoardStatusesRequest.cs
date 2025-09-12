using TaskManagement.Server.Contracts.DtoEnums;

namespace TaskManagement.Server.Contracts.Requests
{
    public sealed class UpdateBoardStatusesRequest
    {
        public TaskStatusDtoEnum[]? Statuses { get; init; }
    }
}
