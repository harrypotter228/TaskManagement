using TaskManagement.Server.Contracts.DtoEnums;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Domains.Enums;

namespace TaskManagement.Server.Services
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(Guid boardId, CreateTaskRequest request);
        Task<TaskDto?> GetTaskByIdAsync(Guid boardId, Guid taskId, Guid? userId = null);
        Task<List<TaskDto>> GetTasksAsync(Guid boardId, Guid? userId = null, TaskStatusEnum? status = null, string? search = null);
        Task<List<TaskDto>> GetTasksSortedByColumnAsync(Guid boardId, Guid userId, TaskStatusDtoEnum status);
        Task<TaskDto> UpdateTaskAsync(Guid boardId, Guid taskId, UpdateTaskRequest request, Guid? userId = null);
        Task<TaskDto> UpdateTaskStatusAsync(Guid boardId, Guid taskId, ChangeTaskStatusRequest request, Guid? userId = null);
        Task DeleteTaskAsync(Guid boardId, Guid taskId);
        Task<DeleteTasksResponse> DeleteTasksBulkAsync(Guid boardId, DeleteTasksRequest request);
    }
}
