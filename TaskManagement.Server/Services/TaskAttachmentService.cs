using TaskManagement.Server.Common.Constants;
using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Common.Extensions;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Services
{
    public class TaskAttachmentService : ITaskAttachmentService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IBoardTaskRepository _boardTaskRepository;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ILogger<TaskAttachmentService> _logger;

        public TaskAttachmentService(
            IBoardRepository boardRepository,
            IBoardTaskRepository boardTaskRepository,
            ITaskItemRepository taskItemRepository,
            ILogger<TaskAttachmentService> logger)
        {
            _boardRepository = boardRepository;
            _boardTaskRepository = boardTaskRepository;
            _taskItemRepository = taskItemRepository;
            _logger = logger;
        }

        public async Task<List<AttachmentDto>> GetTaskAttachmentsAsync(Guid boardId, Guid taskId)
        {
            _logger.LogOperationStart("GetTaskAttachments", new { boardId, taskId });

            try
            {
                if (!await BoardExistsAsync(boardId))
                    throw new NotFoundException("Board not found.");

                if (!await IsTaskInBoardAsync(boardId, taskId))
                    throw new NotFoundException("Task is not in this board.");

                var task = _taskItemRepository.Get(taskId);
                if (task == null)
                    throw new NotFoundException("Task not found.");

                var items = task.Attachments
                    .OrderByDescending(a => a.UploadedAtUtc)
                    .Select(a => ToDto(a))
                    .ToList();

                _logger.LogOperationSuccess("GetTaskAttachments", new { Count = items.Count });
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogOperationError("GetTaskAttachments", ex, new { boardId, taskId });
                throw;
            }
        }

        public async Task<AttachmentDto> CreateAttachmentFromUrlAsync(Guid boardId, Guid taskId, CreateAttachmentFromUrlRequest request)
        {
            if (!await BoardExistsAsync(boardId))
                throw new NotFoundException("Board not found.");

            if (!await IsTaskInBoardAsync(boardId, taskId))
                throw new NotFoundException("Task is not in this board.");

            ValidateCreateAttachmentRequest(request);

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                throw new NotFoundException("Task not found.");

            var attachment = task.AddAttachment(
                fileName: request.FileName.SanitizeFileName(),
                mimeType: request.MimeType,
                url: new Uri(request.Url, UriKind.Absolute),
                uploadedByUserId: request.UploadedByUserId);

            _taskItemRepository.Update(task);

            return ToDto(attachment);
        }

        public async Task<AttachmentDto> UploadAttachmentAsync(Guid boardId, Guid taskId, IFormFile file, Guid uploadedByUserId, IWebHostEnvironment env, HttpRequest http)
        {
            if (!await BoardExistsAsync(boardId))
                throw new NotFoundException("Board not found.");

            if (!await IsTaskInBoardAsync(boardId, taskId))
                throw new NotFoundException("Task is not in this board.");

            ValidateUploadRequest(file, uploadedByUserId);

            var webRoot = string.IsNullOrEmpty(env.WebRootPath)
                ? Path.Combine(AppContext.BaseDirectory, "wwwroot")
                : env.WebRootPath;

            var dir = Path.Combine(webRoot, FileConstants.UploadsPath, taskId.ToString("N"));
            Directory.CreateDirectory(dir);

            var safeName = file.FileName.SanitizeFileName();
            var savePath = Path.Combine(dir, safeName);

            if (file.Length > FileConstants.MaxFileSizeBytes)
                throw new ValidationException($"File too large. Max {FileConstants.MaxFileSizeBytes / (1024 * 1024)} MB.");

            using (var fs = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            // URL public (StaticFiles): /uploads/{taskIdN}/{safeName}
            var relUrl = $"/{FileConstants.UploadsPath}/{taskId:N}/{Uri.EscapeDataString(safeName)}";

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                throw new NotFoundException("Task not found.");

            var attachment = task.AddAttachment(
                fileName: safeName,
                mimeType: file.ContentType ?? "",
                url: new Uri(relUrl, UriKind.Relative),
                uploadedByUserId: uploadedByUserId);

            _taskItemRepository.Update(task);

            return ToDto(attachment);
        }

        public async Task DeleteAttachmentAsync(Guid boardId, Guid taskId, Guid attachmentId, IWebHostEnvironment env)
        {
            if (!await BoardExistsAsync(boardId))
                throw new NotFoundException("Board not found.");

            if (!await IsTaskInBoardAsync(boardId, taskId))
                throw new NotFoundException("Task is not in this board.");

            var task = _taskItemRepository.Get(taskId);
            if (task == null)
                throw new NotFoundException("Task not found.");

            var attachment = task.Attachments.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment == null)
                throw new NotFoundException("Attachment not found.");

            // Delete physical file if it's a local upload
            if (Uri.TryCreate(attachment.Url, UriKind.RelativeOrAbsolute, out var u) && u.IsAbsoluteUri == false
                && attachment.Url.StartsWith($"/{FileConstants.UploadsPath}/", StringComparison.OrdinalIgnoreCase))
            {
                var webRoot = string.IsNullOrEmpty(env.WebRootPath)
                    ? Path.Combine(AppContext.BaseDirectory, "wwwroot")
                    : env.WebRootPath;

                var filePath = Path.Combine(webRoot, attachment.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { /* swallow; file lock etc. */ }
                }
            }

            task.RemoveAttachment(attachmentId);
            _taskItemRepository.Update(task);
        }

        public async Task<bool> BoardExistsAsync(Guid boardId)
        {
            return _boardRepository.Get(boardId) != null;
        }

        public async Task<bool> IsTaskInBoardAsync(Guid boardId, Guid taskId)
        {
            return _boardTaskRepository.Exists(boardId, taskId);
        }

        private void ValidateCreateAttachmentRequest(CreateAttachmentFromUrlRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Url))
                errors["url"] = new[] { "Url is required." };

            if (string.IsNullOrWhiteSpace(request.FileName))
                errors["fileName"] = new[] { "FileName is required." };

            if (string.IsNullOrWhiteSpace(request.MimeType) || !FileConstants.AllowedImageMimeTypes.Contains(request.MimeType))
                errors["mimeType"] = new[] { "Invalid or unsupported image mime type." };

            if (request.UploadedByUserId == Guid.Empty)
                errors["uploadedByUserId"] = new[] { "uploadedByUserId is required." };

            if (errors.Any())
            {
                _logger.LogValidationError("CreateAttachmentFromUrl", errors, request);
                throw new ValidationException("Validation failed.", errors);
            }
        }

        private void ValidateUploadRequest(IFormFile file, Guid uploadedByUserId)
        {
            var errors = new Dictionary<string, string[]>();

            if (file == null || file.Length == 0)
                errors["file"] = new[] { "Image file is required." };

            if (uploadedByUserId == Guid.Empty)
                errors["uploadedByUserId"] = new[] { "uploadedByUserId is required." };

            var mime = file?.ContentType ?? "";
            if (!string.IsNullOrEmpty(mime) && !FileConstants.AllowedImageMimeTypes.Contains(mime))
                errors["file"] = new[] { "Unsupported image type." };

            if (errors.Any())
            {
                _logger.LogValidationError("UploadAttachment", errors, new { FileName = file?.FileName, FileSize = file?.Length, uploadedByUserId });
                throw new ValidationException("Validation failed.", errors);
            }
        }

        private static AttachmentDto ToDto(TaskAttachment attachment) =>
            new(attachment.Id, attachment.TaskId, attachment.FileName, attachment.MimeType, attachment.Url, attachment.UploadedByUserId, attachment.UploadedAtUtc);
    }
}
