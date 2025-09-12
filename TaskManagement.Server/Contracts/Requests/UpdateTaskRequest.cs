using System.ComponentModel.DataAnnotations;
using TaskManagement.Server.Domains.Enums;

namespace TaskManagement.Server.Contracts.Requests
{
    public sealed class UpdateTaskRequest
    {
        [Required] public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        /// <summary>yyyy-MM-dd or null/empty to clear</summary>
        public string? Deadline { get; init; }
        public TaskStatusEnum? Status { get; init; }
    }
}
