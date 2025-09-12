using System.Collections.Concurrent;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Infrastructure.InMemoryRepositories
{
    public class BoardTaskRepository: IBoardTaskRepository
    {
        // BoardId -> Set<TaskId>
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> _boardToTasks = new();
        // TaskId -> Set<BoardId>
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> _taskToBoards = new();

        public void Add(Guid boardId, Guid taskId)
        {
            var tasks = _boardToTasks.GetOrAdd(boardId, _ => new());
            tasks[taskId] = 1;
            var boards = _taskToBoards.GetOrAdd(taskId, _ => new());
            boards[boardId] = 1;
        }

        public void Remove(Guid boardId, Guid taskId)
        {
            if (_boardToTasks.TryGetValue(boardId, out var tasks)) tasks.TryRemove(taskId, out _);
            if (_taskToBoards.TryGetValue(taskId, out var boards)) boards.TryRemove(boardId, out _);
        }

        public bool Exists(Guid boardId, Guid taskId)
            => _boardToTasks.TryGetValue(boardId, out var tasks) && tasks.ContainsKey(taskId);

        public IReadOnlyCollection<Guid> GetTaskIds(Guid boardId)
            => _boardToTasks.TryGetValue(boardId, out var tasks) ? tasks.Keys.ToList() : Array.Empty<Guid>();

        public IReadOnlyCollection<Guid> GetBoardIds(Guid taskId)
            => _taskToBoards.TryGetValue(taskId, out var boards) ? boards.Keys.ToList() : Array.Empty<Guid>();
    }
}
