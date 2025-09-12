namespace TaskManagement.Server.Contracts.Responses
{
    public sealed record DeleteTasksResponse(
    IReadOnlyList<Guid> Removed,
    IReadOnlyList<Guid> NotFound
);
}
