using TaskManagement.Server.Common.Constants;
using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Common.Extensions;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.DtoEnums;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Enums;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IBoardTaskRepository _boardTaskRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public TaskService(
            ITaskItemRepository taskItemRepository,
            IBoardTaskRepository boardTaskRepository,
            IFavoriteRepository favoriteRepository)
        {
            _taskItemRepository = taskItemRepository;
            _boardTaskRepository = boardTaskRepository;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<TaskDto> CreateTaskAsync(Guid boardId, CreateTaskRequest request)
        {
            ValidateCreateTaskRequest(request);

            DateOnly? deadline = null;
            if (!string.IsNullOrWhiteSpace(request.Deadline))
            {
                if (!DateOnly.TryParse(request.Deadline, out var d))
                    throw new ValidationException($"Invalid date format. Use {ValidationConstants.DateFormat}.", 
                        new Dictionary<string, string[]> { ["deadline"] = new[] { $"Invalid date format. Use {ValidationConstants.DateFormat}." } });
                deadline = d;
            }

            var task = new TaskItem(
                request.Name.TrimOrDefault(), 
                request.Description?.TrimOrDefault(), 
                deadline, 
                TaskStatusEnum.ToDo);
            
            task = _taskItemRepository.Create(task);
            _boardTaskRepository.Add(boardId, task.Id);

            return ToDto(task, boardId, isFavorite: false);
        }

        public async Task<TaskDto?> GetTaskByIdAsync(Guid boardId, Guid taskId, Guid? userId = null)
        {
            if (!_boardTaskRepository.Exists(boardId, taskId))
                return null;

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                return null;

            var isFavorite = userId.HasValue && _favoriteRepository.IsFavorite(userId.Value, taskId);
            return ToDto(task, boardId, isFavorite);
        }

        public async Task<List<TaskDto>> GetTasksAsync(Guid boardId, Guid? userId = null, TaskStatusEnum? status = null, string? search = null)
        {
            var taskIds = _boardTaskRepository.GetTaskIds(boardId);
            var items = new List<(TaskItem Task, bool IsFav)>();

            foreach (var id in taskIds)
            {
                var task = _taskItemRepository.Get(id);
                if (task == null) continue;

                if (status.HasValue && task.Status != status.Value) continue;
                if (!string.IsNullOrWhiteSpace(search) &&
                    (task.Name is null || !task.Name.Contains(search, StringComparison.OrdinalIgnoreCase))) continue;

                var isFav = userId.HasValue && _favoriteRepository.IsFavorite(userId.Value, task.Id);
                items.Add((task, isFav));
            }

            return items
                .OrderByDescending(x => x.IsFav)
                .ThenBy(x => x.Task.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => ToDto(x.Task, boardId, x.IsFav))
                .ToList();
        }

        public async Task<List<TaskDto>> GetTasksSortedByColumnAsync(Guid boardId, Guid userId, TaskStatusDtoEnum status)
        {
            var taskIds = _boardTaskRepository.GetTaskIds(boardId);
            var items = new List<(TaskItem Task, bool Fav)>();

            foreach (var id in taskIds)
            {
                var task = _taskItemRepository.Get(id);
                if (task == null) continue;
                if (task.Status != MapTaskStatusEnum(status)) continue;
                
                var fav = _favoriteRepository.IsFavorite(userId, task.Id);
                items.Add((task, fav));
            }

            return items
                .OrderByDescending(x => x.Fav)
                .ThenBy(x => x.Task.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => ToDto(x.Task, boardId, x.Fav))
                .ToList();
        }

        public async Task<TaskDto> UpdateTaskAsync(Guid boardId, Guid taskId, UpdateTaskRequest request, Guid? userId = null)
        {
            if (!_boardTaskRepository.Exists(boardId, taskId))
                throw new NotFoundException("Task is not in this board.");

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                throw new NotFoundException("Task not found.");

            ValidateUpdateTaskRequest(request);

            // Handle deadline
            DateOnly? deadline = null;
            if (!string.IsNullOrWhiteSpace(request.Deadline))
            {
                if (!DateOnly.TryParse(request.Deadline, out var d))
                    throw new ValidationException($"Invalid date format. Use {ValidationConstants.DateFormat}.", 
                        new Dictionary<string, string[]> { ["deadline"] = new[] { $"Invalid date format. Use {ValidationConstants.DateFormat}." } });
                deadline = d;
            }

            // Apply changes
            task.Rename(request.Name.TrimOrDefault());
            task.UpdateDetails(request.Description?.TrimOrDefault(), deadline);
            if (request.Status.HasValue) 
                task.ChangeStatus(request.Status.Value);

            _taskItemRepository.Update(task);

            var isFav = userId.HasValue && _favoriteRepository.IsFavorite(userId.Value, taskId);
            return ToDto(task, boardId, isFav);
        }

        public async Task<TaskDto> UpdateTaskStatusAsync(Guid boardId, Guid taskId, ChangeTaskStatusRequest request, Guid? userId = null)
        {
            if (!_boardTaskRepository.Exists(boardId, taskId))
                throw new NotFoundException("Task is not in this board.");

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                throw new NotFoundException("Task not found.");

            if (!request.Status.HasValue)
                throw new ValidationException("Invalid status.", 
                    new Dictionary<string, string[]> { ["status"] = new[] { "Invalid status." } });

            var newStatus = MapTaskStatusEnum(request.Status.Value);
            if (task.Status != newStatus)
            {
                task.ChangeStatus(newStatus);
                _taskItemRepository.Update(task);
            }

            var isFav = userId.HasValue && _favoriteRepository.IsFavorite(userId.Value, taskId);
            return ToDto(task, boardId, isFav);
        }

        public async Task DeleteTaskAsync(Guid boardId, Guid taskId)
        {
            if (!_boardTaskRepository.Exists(boardId, taskId))
                throw new NotFoundException("Task is not in this board.");

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                throw new NotFoundException("Task not found.");

            var allBoardIds = _boardTaskRepository.GetBoardIds(taskId);
            foreach (var bid in allBoardIds) 
                _boardTaskRepository.Remove(bid, taskId);
            
            _taskItemRepository.Delete(taskId);
        }

        public async Task<DeleteTasksResponse> DeleteTasksBulkAsync(Guid boardId, DeleteTasksRequest request)
        {
            if (request?.TaskIds is null || request.TaskIds.Length == 0)
                throw new ValidationException("TaskIds is required.", 
                    new Dictionary<string, string[]> { ["taskIds"] = new[] { "TaskIds is required." } });

            var removed = new List<Guid>();
            var notFound = new List<Guid>();

            foreach (var id in request.TaskIds.Distinct())
            {
                if (!_boardTaskRepository.Exists(boardId, id)) 
                { 
                    notFound.Add(id); 
                    continue; 
                }
                _boardTaskRepository.Remove(boardId, id);
                removed.Add(id);
            }

            return new DeleteTasksResponse(removed, notFound);
        }

        // Helper methods
        private static TaskDto ToDto(TaskItem task, Guid boardId, bool isFavorite) =>
            new(
                task.Id,
                task.Name,
                task.Description,
                task.Deadline?.ToString("yyyy-MM-dd"),
                MapTaskStatusEnumToDto(task.Status),
                boardId,
                isFavorite
            );

        private static TaskStatusDtoEnum MapTaskStatusEnumToDto(TaskStatusEnum status) =>
            status switch
            {
                TaskStatusEnum.ToDo => TaskStatusDtoEnum.ToDo,
                TaskStatusEnum.InProgress => TaskStatusDtoEnum.InProgress,
                TaskStatusEnum.Done => TaskStatusDtoEnum.Done,
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unhandled status value: {status}")
            };

        private static TaskStatusEnum MapTaskStatusEnum(TaskStatusDtoEnum status) =>
            status switch
            {
                TaskStatusDtoEnum.ToDo => TaskStatusEnum.ToDo,
                TaskStatusDtoEnum.InProgress => TaskStatusEnum.InProgress,
                TaskStatusDtoEnum.Done => TaskStatusEnum.Done,
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unhandled status value: {status}")
            };

        private static void ValidateCreateTaskRequest(CreateTaskRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors["name"] = new[] { "Name is required." };
            else if (request.Name.Length > ValidationConstants.MaxNameLength)
                errors["name"] = new[] { $"Name cannot exceed {ValidationConstants.MaxNameLength} characters." };

            if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > ValidationConstants.MaxDescriptionLength)
                errors["description"] = new[] { $"Description cannot exceed {ValidationConstants.MaxDescriptionLength} characters." };

            if (errors.Any())
                throw new ValidationException("Validation failed.", errors);
        }

        private static void ValidateUpdateTaskRequest(UpdateTaskRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors["name"] = new[] { "Name is required." };
            else if (request.Name.Length > ValidationConstants.MaxNameLength)
                errors["name"] = new[] { $"Name cannot exceed {ValidationConstants.MaxNameLength} characters." };

            if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > ValidationConstants.MaxDescriptionLength)
                errors["description"] = new[] { $"Description cannot exceed {ValidationConstants.MaxDescriptionLength} characters." };

            if (errors.Any())
                throw new ValidationException("Validation failed.", errors);
        }
    }
}
