namespace TaskManagement.Server.Contracts.Dtos
{
    public sealed record AttachmentDto(
    Guid Id,
    Guid TaskId,
    string FileName,
    string MimeType,
    string Url,
    Guid UploadedByUserId,
    DateTime UploadedAtUtc
);
}
