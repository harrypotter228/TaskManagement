using Microsoft.AspNetCore.Mvc;
using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Common.Extensions;
using TaskManagement.Server.Contracts.DtoEnums;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Domains.Enums;
using TaskManagement.Server.Services;

namespace TaskManagement.Server.Endpoints
{
    public static class BoardTasksEndpoints
    {
        public static IEndpointRouteBuilder MapBoardTaskEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/boards/{boardId:guid}/tasks");

            // -------------------------
            // POST /api/v1/boards/{boardId}/tasks  (Users can add new tasks to the task board)
            // -------------------------
            group.MapPost("", async (
                Guid boardId,
                CreateTaskRequest req,
                ITaskService taskService,
                CancellationToken ct) =>
            {
                try
                {
                    var dto = await taskService.CreateTaskAsync(boardId, req);
                    var location = $"/api/v1/boards/{boardId}/tasks/{dto.Id}";
                    return Results.Created(location, dto);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
                catch (ValidationException ex)
                {
                    return EndpointExtensions.CreateValidationProblem(ex.Errors);
                }
            })
            .WithName("Boards_CreateTask")
            .Produces<TaskDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // PUT /api/v1/boards/{boardId}/tasks/{taskId}  (Users can edit tasks and all details)
            // -------------------------
            group.MapPut("{taskId:guid}", async (
                Guid boardId,
                Guid taskId,
                UpdateTaskRequest req,
                ITaskService taskService,
                Guid? userId
            ) =>
            {
                try
                {
                    var dto = await taskService.UpdateTaskAsync(boardId, taskId, req, userId);
                    return Results.Ok(dto);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
                catch (ValidationException ex)
                {
                    return EndpointExtensions.CreateValidationProblem(ex.Errors);
                }
            })
            .WithName("Boards_UpdateTask")
            .Produces<TaskDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // DELETE /api/v1/boards/{boardId}/tasks/{taskId} (Users can delete tasks)
            // -------------------------
            group.MapDelete("{taskId:guid}", async (
                Guid boardId,
                Guid taskId,
                ITaskService taskService,
                string? scope
            ) =>
            {
                try
                {
                    await taskService.DeleteTaskAsync(boardId, taskId);
                    return Results.NoContent();
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
            })
            .WithName("Boards_DeleteTask")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // DELETE /api/v1/boards/{boardId}/tasks/bulk (Users can delete tasks)
            // -------------------------
            group.MapDelete("bulk", async (
                Guid boardId,
                [FromBody] DeleteTasksRequest req,
                ITaskService taskService
            ) =>
            {
                try
                {
                    var response = await taskService.DeleteTasksBulkAsync(boardId, req);
                    return Results.Ok(response);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
                catch (ValidationException ex)
                {
                    return EndpointExtensions.CreateValidationProblem(ex.Errors);
                }
            })
            .WithName("Boards_DeleteTasksBulk")
            .Produces<DeleteTasksResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // GET /api/v1/boards/{boardId}/tasks  (List tasks in board) (Users can sort tasks in each column alphabetically by name + Favorited tasks should be sorted to the top every time)
            // query: userId(optional, for favorites)
            // sort: favorites first (if userId provided) then name A→Z
            // -------------------------
            group.MapGet("", async (
                Guid boardId,
                ITaskService taskService,
                Guid? userId,
                TaskStatusEnum? status,
                string? search
            ) =>
            {
                try
                {
                    var result = await taskService.GetTasksAsync(boardId, userId, status, search);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
            })
            .WithName("Boards_ListTasks")
            .Produces<List<TaskDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // GET /api/v1/boards/{boardId}/tasks/{taskId}  (Users can drill into tasks to see all details)
            // -------------------------
            group.MapGet("{taskId:guid}", async (
                Guid boardId,
                Guid taskId,
                ITaskService taskService,
                Guid? userId
            ) =>
            {
                try
                {
                    var dto = await taskService.GetTaskByIdAsync(boardId, taskId, userId);
                    if (dto == null)
                        return EndpointExtensions.CreateNotFoundResult("Task not found or not in this board.");
                    
                    return Results.Ok(dto);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
            })
            .WithName("Boards_GetTaskById")
            .Produces<TaskDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // PUT /api/v1/boards/{boardId}/tasks/statuses  (Users can add columns to the task board representing different work states (ie. ToDo, In Progress & Done)
            // -------------------------
            group.MapPut("/statuses", async (
                Guid boardId,
                [FromBody] UpdateBoardStatusesRequest req,
                IBoardService boardService
            ) =>
            {
                try
                {
                    var response = await boardService.UpdateStatusesAsync(boardId, req);
                    return Results.Ok(response);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
                catch (ValidationException ex)
                {
                    return EndpointExtensions.CreateValidationProblem(ex.Errors);
                }
            })
            .WithName("Boards_UpdateStatuses")
            .Produces<UpdateBoardStatusesResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // PATCH /api/v1/boards/{boardId}/tasks/{taskId}/status  (Users can move tasks between columns)
            // -------------------------
            group.MapPatch("{taskId:guid}/status", async (
                Guid boardId,
                Guid taskId,
                [FromBody] ChangeTaskStatusRequest req,
                ITaskService taskService,
                Guid? userId
            ) =>
            {
                try
                {
                    var dto = await taskService.UpdateTaskStatusAsync(boardId, taskId, req, userId);
                    return Results.Ok(dto);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
                catch (ValidationException ex)
                {
                    return EndpointExtensions.CreateValidationProblem(ex.Errors);
                }
            })
            .WithName("Boards_UpdateTaskStatus")
            .Produces<TaskDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            // -------------------------
            // GET /api/v1/boards/{boardId}/tasks/sorted  (Get tasks sorted by column)
            // -------------------------
            group.MapGet("sorted", async (
                Guid boardId,
                ITaskService taskService,
                [FromQuery] Guid userId,
                [FromQuery] TaskStatusDtoEnum status
            ) =>
            {
                try
                {
                    var result = await taskService.GetTasksSortedByColumnAsync(boardId, userId, status);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
                catch (ValidationException ex)
                {
                    return EndpointExtensions.CreateValidationProblem(ex.Errors);
                }
            })
            .WithName("Boards_ListTasksSortedByColumn")
            .Produces<List<TaskDto>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }
}
