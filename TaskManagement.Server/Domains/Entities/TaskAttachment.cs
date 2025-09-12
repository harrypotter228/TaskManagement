namespace TaskManagement.Server.Domains.Entities
{
    public sealed class TaskAttachment : IEntity
    {
        public Guid TaskId { get; private set; }
        public string FileName { get; private set; }
        public string MimeType { get; private set; }
        public string Url { get; private set; }
        public Guid UploadedByUserId { get; private set; }
        public DateTime UploadedAtUtc { get; private set; } = DateTime.UtcNow;

        private TaskAttachment() { }

        public TaskAttachment(Guid taskId, string fileName, string mimeType, Uri url, Guid uploadedByUserId)
        {
            TaskId = taskId;
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
            Url = url.ToString();
            UploadedByUserId = uploadedByUserId;
        }
    }
}
