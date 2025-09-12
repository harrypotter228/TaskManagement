using TaskManagement.Server.Domains.Entities;

namespace TaskManagement.Server.Domains.Interfaces.Repositories
{
    public interface ITaskItemRepository
    {
        TaskItem Create(TaskItem task);
        TaskItem? Get(Guid taskId);
        void Update(TaskItem task);
        bool Delete(Guid taskId);
    }
}
