using TaskManagement.Server.Domains.Entities;

namespace TaskManagement.Server.Domains.Interfaces.Repositories
{
    public interface IBoardRepository
    {
        Board Create(Board board);
        Board? Get(Guid boardId);
        IEnumerable<Board> GetAll();
    }
}
