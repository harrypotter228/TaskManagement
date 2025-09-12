using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Services;

namespace TaskManagement.Server.Endpoints
{
    public static class UserFavoritesEndpoints
    {
        public static IEndpointRouteBuilder MapUserFavoriteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/users/{userId:guid}/favorites");

            group.MapPost("{taskId:guid}", async (Guid userId, Guid taskId, IFavoriteService favorites) =>
            {
                if (userId == Guid.Empty) throw new NotFoundException("User not found.");
                await favorites.Favorite(userId, taskId);
                return Results.NoContent();
            });

            group.MapDelete("{taskId:guid}", async (Guid userId, Guid taskId, IFavoriteService favorites) =>
            {
                if (userId == Guid.Empty) throw new NotFoundException("User not found.");
                await favorites.Unfavorite(userId, taskId);
                return Results.NoContent();
            });

            return app;
        }
    }
}
