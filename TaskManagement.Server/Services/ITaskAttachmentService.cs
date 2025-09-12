using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;

namespace TaskManagement.Server.Services
{
    public interface ITaskAttachmentService
    {
        Task<List<AttachmentDto>> GetTaskAttachmentsAsync(Guid boardId, Guid taskId);
        Task<AttachmentDto> CreateAttachmentFromUrlAsync(Guid boardId, Guid taskId, CreateAttachmentFromUrlRequest request);
        Task<AttachmentDto> UploadAttachmentAsync(Guid boardId, Guid taskId, IFormFile file, Guid uploadedByUserId, IWebHostEnvironment env, HttpRequest http);
        Task DeleteAttachmentAsync(Guid boardId, Guid taskId, Guid attachmentId, IWebHostEnvironment env);
        Task<bool> BoardExistsAsync(Guid boardId);
        Task<bool> IsTaskInBoardAsync(Guid boardId, Guid taskId);
    }
}
