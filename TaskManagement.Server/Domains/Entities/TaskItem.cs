using TaskManagement.Server.Domains.Enums;

namespace TaskManagement.Server.Domains.Entities
{
    public sealed class TaskItem: IEntity
    {
        private readonly List<TaskAttachment> _attachments = new();
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DateOnly? Deadline { get; private set; }
        public TaskStatusEnum Status { get; private set; } = TaskStatusEnum.ToDo;

        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

        public IReadOnlyCollection<TaskAttachment> Attachments => _attachments;

        private TaskItem() { }
        public TaskItem(string name, string? description, DateOnly? deadline, TaskStatusEnum status = TaskStatusEnum.ToDo)
        {
            Rename(name);
            UpdateDetails(description, deadline);
            Status = status;
        }

        public void Rename(string name)
        { Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required") : name.Trim(); Touch(); }

        public void UpdateDetails(string? description, DateOnly? deadline)
        {
            Description = description?.Trim();
            Deadline = deadline;
            Touch();
        }

        public void ChangeStatus(TaskStatusEnum newStatus) { if (newStatus != Status) { Status = newStatus; Touch(); } }

        public TaskAttachment AddAttachment(string fileName, string mimeType, Uri url, Guid uploadedByUserId)
        {
            var att = new TaskAttachment(Id, fileName, mimeType, url, uploadedByUserId);
            _attachments.Add(att); Touch();
            return att;
        }

        public void RemoveAttachment(Guid attachmentId) { _attachments.RemoveAll(a => a.Id == attachmentId); Touch(); }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}
