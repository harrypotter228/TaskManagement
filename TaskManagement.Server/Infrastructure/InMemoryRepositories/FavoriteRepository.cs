using System.Collections.Concurrent;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Infrastructure.InMemoryRepositories
{
    public class FavoriteRepository: IFavoriteRepository
    {
        // UserId -> Set<TaskId>
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> _fav = new();

        public void Favorite(Guid userId, Guid taskId)
        { var set = _fav.GetOrAdd(userId, _ => new()); set[taskId] = 1; }

        public void Unfavorite(Guid userId, Guid taskId)
        { if (_fav.TryGetValue(userId, out var set)) set.TryRemove(taskId, out _); }

        public bool IsFavorite(Guid userId, Guid taskId)
        { return _fav.TryGetValue(userId, out var set) && set.ContainsKey(taskId); }

    }
}
