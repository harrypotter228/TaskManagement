using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Common.Extensions;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Services;

namespace TaskManagement.Server.Endpoints
{
    public static class TaskAttachmentsEndpoints
    {

        public static IEndpointRouteBuilder MapTaskAttachmentEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/boards/{boardId:guid}/tasks/{taskId:guid}/attachments");

            // -------------------------
            // POST/api/v1/boards/{boardId}/tasks/{taskId}/attachments/upload (Users can attach images to tasks)
            // -------------------------
            group.MapGet("", async (
                Guid boardId,
                Guid taskId,
                ITaskAttachmentService attachmentService
            ) =>
            {
                try
                {
                    var items = await attachmentService.GetTaskAttachmentsAsync(boardId, taskId);
                    return Results.Ok(items);
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
            })
            .WithName("TaskAttachments_List");

            group.MapPost("", async (
                Guid boardId,
                Guid taskId,
                CreateAttachmentFromUrlRequest req,
                ITaskAttachmentService attachmentService
            ) =>
            {
                try
                {
                    var dto = await attachmentService.CreateAttachmentFromUrlAsync(boardId, taskId, req);
                    var location = $"/api/v1/boards/{boardId}/tasks/{taskId}/attachments/{dto.Id}";
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
            .WithName("TaskAttachments_AddByUrl")
            .Produces<AttachmentDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            // POST (multipart/form-data)
            group.MapPost("upload", async (
                Guid boardId,
                Guid taskId,
                IFormFile file,
                Guid uploadedByUserId,
                IWebHostEnvironment env,
                HttpRequest http,
                ITaskAttachmentService attachmentService
            ) =>
            {
                try
                {
                    var dto = await attachmentService.UploadAttachmentAsync(boardId, taskId, file, uploadedByUserId, env, http);
                    var location = $"/api/v1/boards/{boardId}/tasks/{taskId}/attachments/{dto.Id}";
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
            .DisableAntiforgery()
            .WithName("TaskAttachments_Upload")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<AttachmentDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("{attachmentId:guid}", async (
                Guid boardId,
                Guid taskId,
                Guid attachmentId,
                IWebHostEnvironment env,
                ITaskAttachmentService attachmentService
            ) =>
            {
                try
                {
                    await attachmentService.DeleteAttachmentAsync(boardId, taskId, attachmentId, env);
                    return Results.NoContent();
                }
                catch (NotFoundException ex)
                {
                    return EndpointExtensions.CreateNotFoundResult(ex.Message);
                }
            })
            .WithName("TaskAttachments_Delete")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }
}
