
using System.Threading.Tasks;
using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ITaskItemRepository _taskItemRepository;

        public FavoriteService(ITaskItemRepository taskItemRepository, IFavoriteRepository favoriteRepository)
        {
            _taskItemRepository = taskItemRepository;
            _favoriteRepository = favoriteRepository;
        }

        public async Task Favorite(Guid userId, Guid taskId)
        {
            var task = _taskItemRepository.Get(taskId);
            if (task is null) throw new NotFoundException("Task not found.");
            _favoriteRepository.Favorite(userId, taskId);
        }

        public async Task Unfavorite(Guid userId, Guid taskId)
        {
            var task = _taskItemRepository.Get(taskId);
            if (task is null) throw new NotFoundException("Task not found.");
            _favoriteRepository.Unfavorite(userId, taskId); ;
        }
    }
}
