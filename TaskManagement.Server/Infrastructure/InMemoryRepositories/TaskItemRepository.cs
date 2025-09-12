using System.Collections.Concurrent;
using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Infrastructure.InMemoryRepositories
{
    public class TaskItemRepository: ITaskItemRepository
    {
        private readonly ConcurrentDictionary<Guid, TaskItem> _store = new();
        public TaskItem Create(TaskItem task) { _store[task.Id] = task; return task; }
        public TaskItem? Get(Guid taskId) => _store.TryGetValue(taskId, out var t) ? t : null;
        public void Update(TaskItem task) => _store[task.Id] = task;
        public bool Delete(Guid taskId) => _store.TryRemove(taskId, out _);
    }
}
