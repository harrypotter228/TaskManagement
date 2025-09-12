using TaskManagement.Server.Contracts.DtoEnums;

namespace TaskManagement.Server.Contracts.Requests
{
    public sealed class ChangeTaskStatusRequest
    {
        public TaskStatusDtoEnum? Status { get; init; }
    }
}
