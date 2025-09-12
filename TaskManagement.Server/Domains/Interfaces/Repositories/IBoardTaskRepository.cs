namespace TaskManagement.Server.Domains.Interfaces.Repositories
{
    public interface IBoardTaskRepository
    {
        void Add(Guid boardId, Guid taskId);
        void Remove(Guid boardId, Guid taskId);
        bool Exists(Guid boardId, Guid taskId);
        IReadOnlyCollection<Guid> GetTaskIds(Guid boardId);
        IReadOnlyCollection<Guid> GetBoardIds(Guid taskId);
    }
}
