namespace TaskManagement.Server.Domains.Interfaces.Repositories
{
    public interface IFavoriteRepository
    {
        void Favorite(Guid userId, Guid taskId);
        void Unfavorite(Guid userId, Guid taskId);
        bool IsFavorite(Guid userId, Guid taskId);
    }
}
