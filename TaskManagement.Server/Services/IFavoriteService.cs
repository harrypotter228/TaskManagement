using TaskManagement.Server.Contracts.Dtos;

namespace TaskManagement.Server.Services
{
    public interface IFavoriteService
    {
        Task Favorite(Guid userId, Guid taskId);
        Task Unfavorite(Guid userId, Guid taskId);
    }
}
