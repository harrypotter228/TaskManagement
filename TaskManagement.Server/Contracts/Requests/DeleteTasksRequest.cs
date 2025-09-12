namespace TaskManagement.Server.Contracts.Requests
{
    public sealed class DeleteTasksRequest
    {
        public Guid[] TaskIds { get; init; } = Array.Empty<Guid>();
    }
}
