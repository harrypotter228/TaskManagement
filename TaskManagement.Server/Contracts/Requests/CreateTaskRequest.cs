using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Server.Contracts.Requests
{
    public sealed class CreateTaskRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Deadline { get; init; }
    }
}
