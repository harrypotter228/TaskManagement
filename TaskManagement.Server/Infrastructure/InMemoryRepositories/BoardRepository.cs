using System.Collections.Concurrent;
using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Infrastructure.InMemoryRepositories
{
    public class BoardRepository: IBoardRepository
    {
        private readonly ConcurrentDictionary<Guid, Board> _store = new();
        public Board Create(Board board) { _store[board.Id] = board; return board; }
        public Board? Get(Guid boardId) => _store.TryGetValue(boardId, out var b) ? b : null;
        public IEnumerable<Board> GetAll() => _store.Values;
    }
}
