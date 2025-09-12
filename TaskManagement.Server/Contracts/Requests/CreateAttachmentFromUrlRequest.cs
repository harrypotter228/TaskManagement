using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Server.Contracts.Requests
{
    public sealed class CreateAttachmentFromUrlRequest
    {
        [Required] public string FileName { get; init; } = string.Empty;
        [Required] public string MimeType { get; init; } = string.Empty; // image/png, image/jpeg,...
        [Required] public string Url { get; init; } = string.Empty; // absolute URL
        [Required] public Guid UploadedByUserId { get; init; }
    }
}
